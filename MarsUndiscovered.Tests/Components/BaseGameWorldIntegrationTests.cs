using Castle.MicroKernel.Registration;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Maps;
using MarsUndiscovered.Interfaces;
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

        protected void ProgressiveWorldGenerationWithCustomMap(GameWorld gameWorld, IMapGenerator mapGenerator = null)
        {
            var levelGenerator = new TestLevelGenerator(gameWorld, mapGenerator, 60, 60);

            gameWorld.LevelGenerator = levelGenerator;
            gameWorld.ProgressiveWorldGeneration(null, 1, new WorldGenerationTypeParams(MapType.Outdoor));
        }

        protected void NewGameWithNoMonstersNoItems(GameWorld gameWorld)
        {
            var blankMonsterGenerator = new BlankMonsterGenerator(
                Container.Resolve<IMonsterGenerator>()
            );

            var blankItemGenerator = new BlankItemGenerator(
                Container.Resolve<IItemGenerator>()
            );

            gameWorld.LevelGenerator.MonsterGenerator = blankMonsterGenerator;
            gameWorld.LevelGenerator.ItemGenerator = blankItemGenerator;

            gameWorld.NewGame();

            gameWorld.LevelGenerator.MonsterGenerator = blankMonsterGenerator.OriginalMonsterGenerator;
            gameWorld.LevelGenerator.ItemGenerator = blankItemGenerator.OriginalItemGenerator;
        }

        protected void NewGameWithCustomMapNoMonstersNoItems(GameWorld gameWorld, IMapGenerator mapGenerator = null, ILevelGenerator levelGenerator = null)
        {
            mapGenerator ??= new BlankMapGenerator(gameWorld.GameObjectFactory);

            levelGenerator ??= new TestLevelGenerator(gameWorld, mapGenerator, 60, 60);
            gameWorld.LevelGenerator = levelGenerator;

            var blankMonsterGenerator = new BlankMonsterGenerator(
                Container.Resolve<IMonsterGenerator>()
            );

            var blankItemGenerator = new BlankItemGenerator(
                Container.Resolve<IItemGenerator>()
            );

            gameWorld.LevelGenerator.MonsterGenerator = blankMonsterGenerator;
            gameWorld.LevelGenerator.ItemGenerator = blankItemGenerator;

            gameWorld.NewGame();

            gameWorld.LevelGenerator.MonsterGenerator = blankMonsterGenerator.OriginalMonsterGenerator;
            gameWorld.LevelGenerator.ItemGenerator = blankItemGenerator.OriginalItemGenerator;
        }

        protected void NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(GameWorld gameWorld, IMapGenerator mapGenerator = null, ILevelGenerator levelGenerator = null)
        {
            SetupGameWorldWithCustomMapNoMonstersNoItemsNoExitsNoStructures(gameWorld, mapGenerator, levelGenerator);

            gameWorld.NewGame();
        }

        protected void SetupGameWorldWithCustomMapNoMonstersNoItemsNoExitsNoStructures(GameWorld gameWorld, IMapGenerator mapGenerator = null, ILevelGenerator levelGenerator = null)
        {
            mapGenerator ??= new BlankMapGenerator(gameWorld.GameObjectFactory);

            levelGenerator ??= new TestLevelGenerator(gameWorld, mapGenerator, 60, 60);
            gameWorld.LevelGenerator = levelGenerator;

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

        protected Item SpawnItemAndEquip(GameWorld gameWorld, ItemType itemType)
        {
            var item = SpawnItemAndAddToInventory(_gameWorld, itemType);
            gameWorld.Inventory.Equip(item);
            return item;
        }
        
        protected Item SpawnItemAndAddToInventory(GameWorld gameWorld, ItemType itemType)
        {
            gameWorld.SpawnItem(new SpawnItemParams().WithItemType(itemType).AtPosition(gameWorld.Player.Position));
            var item = gameWorld.Items.Last().Value;
            gameWorld.Inventory.Add(item);
            gameWorld.CurrentMap.RemoveEntity(item);
            item.Position = Point.None;
            return item;
        }
    }
}
