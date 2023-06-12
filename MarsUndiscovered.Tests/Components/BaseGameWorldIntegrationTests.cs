using System.ComponentModel.Design;
using Castle.MicroKernel.Registration;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Maps;
using MarsUndiscovered.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Point = SadRogue.Primitives.Point;

namespace MarsUndiscovered.Tests.Components
{
    [TestClass]
    public abstract class BaseGameWorldIntegrationTests : BaseIntegrationTest
    {
        protected TestGameWorld _gameWorld;
        protected IGameProvider _gameProvider;

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
            _gameProvider = Container.Resolve<IGameProvider>();

            var testGame = new TestGame();
            _gameProvider.Game = testGame;
            
            var services = new ServiceContainer();
            var graphicsService = new TestGraphicsDeviceService();
            services.AddService(typeof(IGraphicsDeviceService), graphicsService);
            
            testGame.Content = new ContentManager(services, "Content");
            testGame.GraphicsDevice = graphicsService.GraphicsDevice;

            var gameServiceContainer = new GameServiceContainer();
            gameServiceContainer.AddService(graphicsService);
            
            testGame.Services = gameServiceContainer;
        }
        
        [TestCleanup]
        public override void TearDown()
        {
            base.TearDown();
            _gameProvider.Game.Dispose();
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
