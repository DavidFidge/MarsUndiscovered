using FrigidRogue.MonoGame.Core.Components;

using MarsUndiscovered.Components;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Tests.Components;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SadRogue.Primitives;

namespace MarsUndiscovered.Tests.Commands
{
    [TestClass]
    public class ChangeMapCommandTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        public void ChangeMapCommand_Should_Change_Map()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems();
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

        [TestMethod]
        public void ChangeMapCommand_Should_Change_Map_And_Put_Player_In_Square_Close_To_Landing_Position_When_Landing_Position_Is_Not_Walkable()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems(new SpecificMapGenerator(_gameWorld.GameObjectFactory, _gameWorld.MapGenerator, new List<Point> { new Point(0, 0) }));
            var oldMap = _gameWorld.CurrentMap;
            _gameWorld.Player.Position = new Point(0, 1);

            var mapExit = _gameWorld.MapExits.Values.First(me => me.CurrentMap.Equals(_gameWorld.CurrentMap));
            var destinationMap = (MarsMap)mapExit.Destination.CurrentMap;

            // spawn a monster across first 3 columns (excluding the location of the exit).  This will block the landing position and force a new position
            // to be searched for.
            for (var y = 0; y < MarsMap.MapHeight; y++)
            {
                for (var x = 0; x < 3; x++)
                {
                    if (!(x == 0 && y == 0))
                        _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(new Point(x, y)).OnMap(destinationMap.Id));
                }
            }

            var commandFactory = Container.Resolve<ICommandFactory>();

            var changeMapCommand = commandFactory.CreateChangeMapCommand(_gameWorld);
            changeMapCommand.Initialise(_gameWorld.Player, mapExit);

            // Act
            var result = changeMapCommand.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.Success, result.Result);
            Assert.AreNotEqual(oldMap, _gameWorld.Player.CurrentMap);
            Assert.AreEqual(mapExit.Destination.CurrentMap, _gameWorld.Player.CurrentMap);
            Assert.AreNotEqual(mapExit.Destination.LandingPosition, _gameWorld.Player.Position);

            // Player should end up in a location close to landing position which is not on top of blocking objects such as monsters.
            // This will be at worst a diagonal line to the point 3, 3
            Assert.AreEqual(3, _gameWorld.Player.Position.X);
            Assert.IsTrue(_gameWorld.Player.Position.Y <= 3);
        }
    }
}
