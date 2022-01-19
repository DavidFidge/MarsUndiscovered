using System.Linq;

using FrigidRogue.MonoGame.Core.Components;

using MarsUndiscovered.Components.Factories;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SadRogue.Primitives;

namespace MarsUndiscovered.Tests.Components.GameWorldTests
{
    [TestClass]
    public class ChangeMapCommandTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        public void ChangeMapCommand_Should_Change_Map()
        {
            // Arrange
            NewGameWithNoWallsNoMonstersNoItems();
            var oldMap = _gameWorld.CurrentMap;
            _gameWorld.Player.Position = new Point(0, 0);

            var mapExit = _gameWorld.MapExits.Values.First(me => me.CurrentMap.Equals(_gameWorld.CurrentMap));

            // Ensure landing position is different from current position so we know position has been swapped
            if (_gameWorld.Player.Position == mapExit.Destination.LandingPosition)
                _gameWorld.Player.Position = new Point(0, 1);

            var commandFactory = Container.Resolve<ICommandFactory>();

            var changeMapCommand = commandFactory.CreateChangeMapCommand(_gameWorld);
            changeMapCommand.Initialise(_gameWorld.Player, mapExit);

            // Act
            var result = changeMapCommand.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.Success, result.Result);
            Assert.AreNotEqual(oldMap, _gameWorld.Player.CurrentMap);
            Assert.AreEqual(mapExit.Destination.CurrentMap, _gameWorld.Player.CurrentMap);
            Assert.AreEqual(mapExit.Destination.LandingPosition, _gameWorld.Player.Position);
        }
    }
}