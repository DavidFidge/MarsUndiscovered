using System.Linq;

using MarsUndiscovered.Components;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Components.Maps;
using MarsUndiscovered.Interfaces;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MarsUndiscovered.Tests.Components.GameWorldTests
{
    [TestClass]
    public abstract class BaseGameWorldIntegrationTests : BaseIntegrationTest
    {
        protected GameWorld _gameWorld;

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();

            _gameWorld = (GameWorld)Container.Resolve<IGameWorld>();
        }

        protected void NewGameWithNoWalls()
        {
            // Arrange
            var blankMapGeneration = new BlankMapGenerator(
                Container.Resolve<IGameObjectFactory>(),
                Container.Resolve<IMapGenerator>()
            );

            _gameWorld.MapGenerator = blankMapGeneration;
            _gameWorld.NewGame();
        }

        protected void NewGameWithNoMonstersNoItems()
        {
            // Arrange
            var blankMonsterGenerator = new BlankMonsterGenerator(
                Container.Resolve<IMonsterGenerator>()
            );

            var blankItemGenerator = new BlankItemGenerator(
                Container.Resolve<IItemGenerator>()
            );

            _gameWorld.MonsterGenerator = blankMonsterGenerator;
            _gameWorld.ItemGenerator = blankItemGenerator;
            _gameWorld.NewGame();
            _gameWorld.MonsterGenerator = blankMonsterGenerator.OriginalMonsterGenerator;
            _gameWorld.ItemGenerator = blankItemGenerator.OriginalItemGenerator;
        }

        protected void NewGameWithNoWallsNoMonstersNoItems()
        {
            // Arrange
            var blankMapGeneration = new BlankMapGenerator(
                Container.Resolve<IGameObjectFactory>(),
                Container.Resolve<IMapGenerator>()
            );

            var blankMonsterGenerator = new BlankMonsterGenerator(
                Container.Resolve<IMonsterGenerator>()
            );

            var blankItemGenerator = new BlankItemGenerator(
                Container.Resolve<IItemGenerator>()
            );

            _gameWorld.MapGenerator = blankMapGeneration;
            _gameWorld.MonsterGenerator = blankMonsterGenerator;
            _gameWorld.ItemGenerator = blankItemGenerator;
            _gameWorld.NewGame();
            _gameWorld.MonsterGenerator = blankMonsterGenerator.OriginalMonsterGenerator;
            _gameWorld.ItemGenerator = blankItemGenerator.OriginalItemGenerator;
        }
    }
}