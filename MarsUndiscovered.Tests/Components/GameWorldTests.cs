using System.Linq;

using FrigidRogue.TestInfrastructure;

using MarsUndiscovered.Components;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Tests.ViewModels
{
    [TestClass]
    public class GameWorldTests : BaseTest
    {
        private GameWorld _gameWorld;

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();

            _gameWorld = new GameWorld();
        }

        [TestMethod]
        public void Should_Add_To_LastCommands_When_Command_Executes()
        {
            // Arrange
            _gameWorld.Generate();

            // Act
            var wallsFloors = (ArrayView<bool>)_gameWorld.AllComponents.Single(s => s.Tag == "WallFloor").Component;
            var wallsFloors2 = _gameWorld.Generator.Context.GetFirst<ArrayView<bool>>();

            // Assert
        }
    }
}