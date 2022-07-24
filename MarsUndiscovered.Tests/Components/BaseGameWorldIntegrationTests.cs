using System.Linq;

using Castle.MicroKernel.Registration;

using MarsUndiscovered.Components;
using MarsUndiscovered.Components.Maps;
using MarsUndiscovered.Interfaces;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadRogue.Primitives;

namespace MarsUndiscovered.Tests.Components
{
    [TestClass]
    public abstract class BaseGameWorldIntegrationTests : BaseIntegrationTest
    {
        protected TestGameWorld _gameWorld;

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();

            Container.Register(
                Component.For<IGameWorld>()
                    .ImplementedBy<TestGameWorld>()
                    .LifestyleTransient()
                    .IsDefault()
                );

            _gameWorld = (TestGameWorld)Container.Resolve<IGameWorld>();
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
        
        protected Item SpawnItemAndEquip(ItemType itemType)
        {
            var item = SpawnItemAndAddToInventory(itemType);
            _gameWorld.Inventory.Equip(item);
            return item;
        }
        
        protected Item SpawnItemAndAddToInventory(ItemType itemType)
        {
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(itemType).AtPosition(_gameWorld.Player.Position));
            var item = _gameWorld.Items.Last().Value;
            _gameWorld.Inventory.Add(item);
            _gameWorld.CurrentMap.RemoveEntity(item);
            item.Position = Point.None;
            return item;
        }
    }
}