using System.Linq;

using FrigidRogue.MonoGame.Core.Components;

using MarsUndiscovered.Commands;
using MarsUndiscovered.Components;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Components.Maps;
using MarsUndiscovered.Interfaces;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SadRogue.Primitives;

namespace MarsUndiscovered.Tests.Components.GameWorldTests
{
    [TestClass]
    public class BaseGameWorldIntegrationTests : BaseIntegrationTest
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

        protected void NewGameWithNoWallsNoMonsters()
        {
            // Arrange
            var blankMapGeneration = new BlankMapGenerator(
                Container.Resolve<IGameObjectFactory>(),
                Container.Resolve<IMapGenerator>()
            );

            var blankMonsterGenerator = new BlankMonsterGenerator(
                Container.Resolve<IMonsterGenerator>()
            );

            _gameWorld.MapGenerator = blankMapGeneration;
            _gameWorld.MonsterGenerator = blankMonsterGenerator;
            _gameWorld.NewGame();
            _gameWorld.MonsterGenerator = blankMonsterGenerator.OriginalMonsterGenerator;
        }
    }
}