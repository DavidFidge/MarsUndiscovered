using Castle.MicroKernel.Registration;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Maps;
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
            var levelGenerator = new TestLevelGenerator(_gameWorld, Container, mapGenerator);
            _gameWorld.LevelGenerator = levelGenerator;

            _gameWorld.NewGame();
        }
        
        protected void ProgressiveWorldGenerationWithCustomMap(IMapGenerator mapGenerator = null)
        {
            var levelGenerator = new TestLevelGenerator(_gameWorld, Container, mapGenerator);

            _gameWorld.LevelGenerator = levelGenerator;
            _gameWorld.ProgressiveWorldGeneration(null, 1, new WorldGenerationTypeParams(MapType.Outdoor));
        }

        protected void NewGameWithNoMonstersNoItems()
        {
            var blankMonsterGenerator = new BlankMonsterGenerator(
                Container.Resolve<IMonsterGenerator>()
            );

            var blankItemGenerator = new BlankItemGenerator(
                Container.Resolve<IItemGenerator>()
            );

            _gameWorld.LevelGenerator.MonsterGenerator = blankMonsterGenerator;
            _gameWorld.LevelGenerator.ItemGenerator = blankItemGenerator;

            _gameWorld.NewGame();

            _gameWorld.LevelGenerator.MonsterGenerator = blankMonsterGenerator.OriginalMonsterGenerator;
            _gameWorld.LevelGenerator.ItemGenerator = blankItemGenerator.OriginalItemGenerator;
        }

        protected void NewGameWithCustomMapNoMonstersNoItems(IMapGenerator mapGenerator = null)
        {
            mapGenerator ??= new BlankMapGenerator(_gameWorld.GameObjectFactory);

            _gameWorld.LevelGenerator.MapGenerator = mapGenerator;

            var blankMonsterGenerator = new BlankMonsterGenerator(
                Container.Resolve<IMonsterGenerator>()
            );

            var blankItemGenerator = new BlankItemGenerator(
                Container.Resolve<IItemGenerator>()
            );

            _gameWorld.LevelGenerator.MonsterGenerator = blankMonsterGenerator;
            _gameWorld.LevelGenerator.ItemGenerator = blankItemGenerator;

            _gameWorld.NewGame();

            _gameWorld.LevelGenerator.MonsterGenerator = blankMonsterGenerator.OriginalMonsterGenerator;
            _gameWorld.LevelGenerator.ItemGenerator = blankItemGenerator.OriginalItemGenerator;
        }

        protected void NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(IMapGenerator mapGenerator = null)
        {
            SetupGameWorldWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld, mapGenerator);

            _gameWorld.NewGame();
        }

        protected void SetupGameWorldWithCustomMapNoMonstersNoItemsNoExitsNoStructures(GameWorld gameWorld, IMapGenerator mapGenerator = null)
        {
            mapGenerator ??= new BlankMapGenerator(gameWorld.GameObjectFactory);

            gameWorld.LevelGenerator.MapGenerator = mapGenerator;

            var blankMonsterGenerator = new BlankMonsterGenerator(
                Container.Resolve<IMonsterGenerator>()
            );

            var blankItemGenerator = new BlankItemGenerator(
                Container.Resolve<IItemGenerator>()
            );

            var blankMapExitGenerator = new BlankMapExitGenerator(
                Container.Resolve<IMapExitGenerator>()
            );

            var blankShipGenerator = new BlankShipGenerator();
            var blankMiningFacilityGenerator = new BlankMiningFacilityGenerator();

            gameWorld.LevelGenerator.MonsterGenerator = blankMonsterGenerator;
            gameWorld.LevelGenerator.ItemGenerator = blankItemGenerator;
            gameWorld.LevelGenerator.MapExitGenerator = blankMapExitGenerator;
            gameWorld.LevelGenerator.ShipGenerator = blankShipGenerator;
            gameWorld.LevelGenerator.MiningFacilityGenerator = blankMiningFacilityGenerator;
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
