using MarsUndiscovered.Components;
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

        protected void NewGameWithCustomMap(IMapGenerator mapGenerator = null)
        {
            // Arrange
            if (mapGenerator == null)
            {
                mapGenerator = new BlankMapGenerator(
                    _gameWorld.GameObjectFactory,
                    Container.Resolve<IMapGenerator>()
                );
            }

            _gameWorld.MapGenerator = mapGenerator;

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

        protected void NewGameWithCustomMapNoMonstersNoItems(IMapGenerator mapGenerator = null)
        {
            // Arrange
            if (mapGenerator == null)
            {
                mapGenerator = new BlankMapGenerator(
                    _gameWorld.GameObjectFactory,
                    Container.Resolve<IMapGenerator>()
                );
            }

            var blankMonsterGenerator = new BlankMonsterGenerator(
                Container.Resolve<IMonsterGenerator>()
            );

            var blankItemGenerator = new BlankItemGenerator(
                Container.Resolve<IItemGenerator>()
            );

            _gameWorld.MapGenerator = mapGenerator ?? mapGenerator;
            _gameWorld.MonsterGenerator = blankMonsterGenerator;
            _gameWorld.ItemGenerator = blankItemGenerator;

            _gameWorld.NewGame();

            _gameWorld.MonsterGenerator = blankMonsterGenerator.OriginalMonsterGenerator;
            _gameWorld.ItemGenerator = blankItemGenerator.OriginalItemGenerator;
        }

        protected void NewGameWithCustomMapNoMonstersNoItemsNoExits(IMapGenerator mapGenerator = null)
        {
            // Arrange
            if (mapGenerator == null)
            {
                mapGenerator = new BlankMapGenerator(
                    _gameWorld.GameObjectFactory,
                    Container.Resolve<IMapGenerator>()
                );
            }

            var blankMonsterGenerator = new BlankMonsterGenerator(Container.Resolve<IMonsterGenerator>());
            var blankItemGenerator = new BlankItemGenerator(Container.Resolve<IItemGenerator>());
            var blankMapExitGenerator = new BlankMapExitGenerator(Container.Resolve<IMapExitGenerator>());

            _gameWorld.MapGenerator = mapGenerator;
            _gameWorld.MonsterGenerator = blankMonsterGenerator;
            _gameWorld.ItemGenerator = blankItemGenerator;
            _gameWorld.MapExitGenerator = blankMapExitGenerator;

            _gameWorld.NewGame();

            _gameWorld.MonsterGenerator = blankMonsterGenerator.OriginalMonsterGenerator;
            _gameWorld.ItemGenerator = blankItemGenerator.OriginalItemGenerator;
            _gameWorld.MapExitGenerator = blankMapExitGenerator.OriginalMapExitGenerator;
        }
    }
}