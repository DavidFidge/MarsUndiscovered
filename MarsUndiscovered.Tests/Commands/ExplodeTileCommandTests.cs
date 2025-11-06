using FrigidRogue.MonoGame.Core.Components;

using GoRogue.Random;

using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.Maps;
using MarsUndiscovered.Interfaces;
using MarsUndiscovered.Tests.Components;

using SadRogue.Primitives;

using Point = SadRogue.Primitives.Point;

namespace MarsUndiscovered.Tests.Commands
{
    [TestClass]
    public class ExplodeTileCommandTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        public void ApplyExplodeTileCommand_Should_Destroy_Wall()
        {
            // Arrange
            // Ensure a rubble is always created
            _gameWorld.TestContextualEnhancedRandom.KnownSeries.Add(nameof(ExplodeTileCommand), new ShaiRandom.Generators.KnownSeriesRandom(boolSeries: [true]));

            var lines = new[]
            {
                "...",
                ".#.",
                "..."
            };

            var mapTemplate = new MapTemplate(lines);
            
            var mapGenerator = new SpecificMapGenerator(
                _gameWorld.GameObjectFactory,
                mapTemplate
                    .Where(m => m.Char == '#')
                    .Select(m => m.Point).ToList());
            
            NewGameWithTestLevelGenerator(
                _gameWorld,
                mapGenerator,
                playerPosition: new Point(0, 0),
                mapWidth: mapTemplate.Bounds.Width,
                mapHeight: mapTemplate.Bounds.Height);

            var spawnEnvironmentalEffectParams = new SpawnEnvironmentalEffectParams()
                .WithEnvironmentalEffectType(EnvironmentalEffectType.MissileTargetType)
                .AtPosition(new Point(1, 1))
                .OnMap(_gameWorld.CurrentMap.Id);
            
            _gameWorld.SpawnEnvironmentalEffect(spawnEnvironmentalEffectParams);

            var environmentalEffect = spawnEnvironmentalEffectParams.Result;

            var commandFactory = Container.Resolve<ICommandCollection>();

            var explodeTileCommand = commandFactory.CreateCommand<ExplodeTileCommand>(_gameWorld);
            explodeTileCommand.Initialise(new Point(1, 1), environmentalEffect.Damage, environmentalEffect);
    
            // Act
            var result = explodeTileCommand.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.Success, result.Result);

            var gameObjects = _gameWorld.CurrentMap.GetObjectsAt(new Point(1, 1)).ToList();

            Assert.AreEqual(3, gameObjects.Count);
            
            Assert.IsNotNull(gameObjects.FirstOrDefault(f => f is EnvironmentalEffect));
            var floor = (Floor)gameObjects.First(f => f is Floor);
            Assert.IsNotNull(floor);
            Assert.AreEqual(FloorType.RockFloor, floor.FloorType);
            Assert.IsNotNull(gameObjects.FirstOrDefault(f => f is Feature feature && feature.FeatureType == FeatureType.RubbleType));

            Assert.IsFalse(result.Command.EndsPlayerTurn);
            Assert.IsFalse(result.Command.RequiresPlayerInput);
            Assert.IsFalse(result.Command.InterruptsMovement);
        } 
        
        [TestMethod]
        public void ApplyExplodeTileCommand_Should_Kill_Monster()
        {
            // Arrange
            var lines = new[]
            {
                "...",
                "...",
                "..."
            };

            var mapTemplate = new MapTemplate(lines);
            
            var mapGenerator = new SpecificMapGenerator(
                _gameWorld.GameObjectFactory,
                mapTemplate
                    .Where(m => m.Char == '#')
                    .Select(m => m.Point).ToList());
            
            NewGameWithTestLevelGenerator(
                _gameWorld,
                mapGenerator,
                playerPosition: new Point(0, 0),
                mapWidth: mapTemplate.Bounds.Width,
                mapHeight: mapTemplate.Bounds.Height);

            var monster1Params = new SpawnMonsterParams()
                .AtPosition(new Point(1, 1))
                .WithBreed(Breed.Breeds.First().Value)
                .OnMap(_gameWorld.CurrentMap.Id);
            
            _gameWorld.SpawnMonster(monster1Params);

            var monster1 = monster1Params.Result;
            
            var spawnEnvironmentalEffectParams = new SpawnEnvironmentalEffectParams()
                .WithEnvironmentalEffectType(EnvironmentalEffectType.MissileTargetType)
                .AtPosition(new Point(1, 1))
                .OnMap(_gameWorld.CurrentMap.Id);
            
            _gameWorld.SpawnEnvironmentalEffect(spawnEnvironmentalEffectParams);

            var environmentalEffect = spawnEnvironmentalEffectParams.Result;
            
            var commandFactory = Container.Resolve<ICommandCollection>();

            var explodeTileCommand = commandFactory.CreateCommand<ExplodeTileCommand>(_gameWorld);
            explodeTileCommand.Initialise(new Point(1, 1), environmentalEffect.Damage, environmentalEffect);
    
            // Act
            var result = explodeTileCommand.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.Success, result.Result);

            Assert.AreEqual(
                $"An explosion blasts {monster1.GetSentenceName(true, false)} for {environmentalEffect.Damage} dammage",
                result.Messages[0]);
            
            Assert.AreEqual(1, result.SubsequentCommands.Count);
            var deathCommand = result.SubsequentCommands.First() as DeathCommand;
            Assert.IsNotNull(deathCommand);
            
            Assert.IsFalse(result.Command.EndsPlayerTurn);
            Assert.IsFalse(result.Command.RequiresPlayerInput);
            Assert.IsFalse(result.Command.InterruptsMovement);
        }

        [TestMethod]
        public void Missile_Environmental_Effect_Should_Explode_Tile_After_Duration()
        {
            // Arrange
            // Ensure a rubble is always created
            _gameWorld.TestContextualEnhancedRandom.KnownSeries.Add(nameof(ExplodeTileCommand), new ShaiRandom.Generators.KnownSeriesRandom(boolSeries: [true]));

            var lines = new[]
            {
                "...",
                "...",
                "..."
            };

            var mapTemplate = new MapTemplate(lines);

            var mapGenerator = new SpecificMapGenerator(
                _gameWorld.GameObjectFactory,
                mapTemplate
                    .Where(m => m.Char == '#')
                    .Select(m => m.Point).ToList());

            NewGameWithTestLevelGenerator(
                _gameWorld,
                mapGenerator,
                playerPosition: new Point(0, 0),
                mapWidth: mapTemplate.Bounds.Width,
                mapHeight: mapTemplate.Bounds.Height);

            var spawnEnvironmentalEffectParams = new SpawnEnvironmentalEffectParams()
                .WithEnvironmentalEffectType(EnvironmentalEffectType.MissileTargetType)
                .AtPosition(new Point(1, 1))
                .OnMap(_gameWorld.CurrentMap.Id);

            _gameWorld.SpawnEnvironmentalEffect(spawnEnvironmentalEffectParams);

            var environmentalEffect = spawnEnvironmentalEffectParams.Result;

            environmentalEffect.Duration = 2;

            // Act
            _gameWorld.TestNextTurn().ToList();

            Assert.AreEqual(1, environmentalEffect.Duration);

            var noResult = _gameWorld.CommandCollection.GetLastCommand<ExplodeTileCommand>();
            Assert.IsNull(noResult);

            _gameWorld.TestNextTurn().ToList();

            Assert.AreEqual(0, environmentalEffect.Duration);

            var result = _gameWorld.CommandCollection.GetLastCommand<ExplodeTileCommand>().CommandResult;

            // Assert
            Assert.AreEqual(CommandResultEnum.Success, result.Result);

            Assert.AreEqual(
                $"There was an explosion nearby",
                result.Messages[0]);

            var features = _gameWorld.CurrentMap.GetObjectsAt<Feature>(new Point(1, 1)).ToList();
            Assert.AreEqual(1, features.Count);

            Assert.AreEqual(FeatureType.RubbleType, features.First().FeatureType);

            Assert.IsFalse(result.Command.EndsPlayerTurn);
            Assert.IsFalse(result.Command.RequiresPlayerInput);
            Assert.IsFalse(result.Command.InterruptsMovement);
        }

        [TestMethod]
        public void LastSeenEntity_Which_Has_Since_Been_Exploded_Should_Still_Show_Old_Game_Object()
        {
            // Arrange
            // Ensure a rubble is always created
            _gameWorld.TestContextualEnhancedRandom.KnownSeries.Add(nameof(ExplodeTileCommand), new ShaiRandom.Generators.KnownSeriesRandom(boolSeries: [true]));

            var lines = new[]
            {
                ".#.",
                ".#.",
                "..."
            };

            var mapTemplate = new MapTemplate(lines);

            var mapGenerator = new SpecificMapGenerator(
                _gameWorld.GameObjectFactory,
                mapTemplate
                    .Where(m => m.Char == '#')
                    .Select(m => m.Point).ToList());

            NewGameWithTestLevelGenerator(
                _gameWorld,
                mapGenerator,
                playerPosition: new Point(0, 0),
                mapWidth: mapTemplate.Bounds.Width,
                mapHeight: mapTemplate.Bounds.Height);

            var commandFactory = Container.Resolve<ICommandCollection>();

            // Act and Assert
            _gameWorld.MoveRequest(Direction.Down);
            _gameWorld.MoveRequest(Direction.DownRight);

            var lastSeenGameObjects = _gameWorld.GetLastSeenGameObjectsAtPosition(new Point(1, 0));
            var lastSeenWall = lastSeenGameObjects.First(o => o is Wall);
            var currentWall = _gameWorld.CurrentMap.GetObjectAt<Wall>(new Point(1, 0));

            Assert.AreEqual(lastSeenWall, currentWall);

            var spawnEnvironmentalEffectParams = new SpawnEnvironmentalEffectParams()
                .WithEnvironmentalEffectType(EnvironmentalEffectType.MissileTargetType)
                .AtPosition(new Point(1, 0))
                .OnMap(_gameWorld.CurrentMap.Id);

            _gameWorld.SpawnEnvironmentalEffect(spawnEnvironmentalEffectParams);

            var environmentalEffect = spawnEnvironmentalEffectParams.Result;

            environmentalEffect.Duration = 1;

            _gameWorld.MoveRequest(Direction.None);

            var newCurrentWall = _gameWorld.CurrentMap.GetObjectAt<Wall>(new Point(1, 0));

            Assert.IsNull(newCurrentWall);

            var lastSeenGameObjectsAfterExplode = _gameWorld.GetLastSeenGameObjectsAtPosition(new Point(1, 0));
            var lastSeenWallAfterExplode = lastSeenGameObjectsAfterExplode.First(o => o is Wall);

            // Last seen is as it was since player only has fog of war on the tile until they see it again
            Assert.AreEqual(lastSeenWall, lastSeenWallAfterExplode);

            // Ensure the last seen game object has been cleared after the player sees it again
            _gameWorld.MoveRequest(Direction.UpRight);

            var lastSeenGameObjectsAfterSeeingAgain = _gameWorld.GetLastSeenGameObjectsAtPosition(new Point(1, 0));
            var lastSeenWallAfterSeeingAgain = lastSeenGameObjectsAfterSeeingAgain.FirstOrDefault(o => o is Wall);

            Assert.IsNull(lastSeenWallAfterSeeingAgain);
        }

        [TestMethod]
        public void LastSeenEntity_Which_Has_Since_Been_Exploded_Should_Still_Show_Old_Game_Object_After_Save_And_Load()
        {
            // Arrange
            // Ensure a rubble is always created
            _gameWorld.TestContextualEnhancedRandom.KnownSeries.Add(nameof(ExplodeTileCommand), new ShaiRandom.Generators.KnownSeriesRandom(boolSeries: [true]));

            var lines = new[]
            {
                ".#.",
                ".#.",
                "..."
            };

            var mapTemplate = new MapTemplate(lines);

            var mapGenerator = new SpecificMapGenerator(
                _gameWorld.GameObjectFactory,
                mapTemplate
                    .Where(m => m.Char == '#')
                    .Select(m => m.Point).ToList());

            NewGameWithTestLevelGenerator(
                _gameWorld,
                mapGenerator,
                playerPosition: new Point(0, 0),
                mapWidth: mapTemplate.Bounds.Width,
                mapHeight: mapTemplate.Bounds.Height);

            var commandFactory = Container.Resolve<ICommandCollection>();

            // Act and Assert
            _gameWorld.MoveRequest(Direction.Down);
            _gameWorld.MoveRequest(Direction.DownRight);

            var lastSeenGameObjects = _gameWorld.GetLastSeenGameObjectsAtPosition(new Point(1, 0));
            var lastSeenWall = lastSeenGameObjects.First(o => o is Wall);
            var currentWall = _gameWorld.CurrentMap.GetObjectAt<Wall>(new Point(1, 0));

            Assert.AreEqual(lastSeenWall, currentWall);

            var spawnEnvironmentalEffectParams = new SpawnEnvironmentalEffectParams()
                .WithEnvironmentalEffectType(EnvironmentalEffectType.MissileTargetType)
                .AtPosition(new Point(1, 0))
                .OnMap(_gameWorld.CurrentMap.Id);

            _gameWorld.SpawnEnvironmentalEffect(spawnEnvironmentalEffectParams);

            var environmentalEffect = spawnEnvironmentalEffectParams.Result;

            environmentalEffect.Duration = 1;

            _gameWorld.MoveRequest(Direction.None);

            var newCurrentWall = _gameWorld.CurrentMap.GetObjectAt<Wall>(new Point(1, 0));

            Assert.IsNull(newCurrentWall);

            var lastSeenGameObjectsAfterExplode = _gameWorld.GetLastSeenGameObjectsAtPosition(new Point(1, 0));
            var lastSeenWallAfterExplode = lastSeenGameObjectsAfterExplode.First(o => o is Wall);

            // Last seen is as it was since player only has fog of war on the tile until they see it again
            Assert.AreEqual(lastSeenWall, lastSeenWallAfterExplode);

            _gameWorld.SaveGame("TestShouldSaveThenLoad", true);

            // Act
            var newGameWorld = (GameWorld)Container.Resolve<IGameWorld>();
            newGameWorld.LoadGame("TestShouldSaveThenLoad");

            // Assert
            var lastSeenGameObjectsAfterLoad = newGameWorld.GetLastSeenGameObjectsAtPosition(new Point(1, 0));
            var lastSeenWallAfterLoad = lastSeenGameObjectsAfterLoad.First(o => o is Wall);

            // Last seen is still loaded even though wall has been destroyed
            Assert.AreEqual(lastSeenWall.ID, lastSeenWallAfterLoad.ID);

            // Ensure the last seen game object has been cleared after the player sees it again
            newGameWorld.MoveRequest(Direction.UpRight);

            var lastSeenGameObjectsAfterSeeingAgain = newGameWorld.GetLastSeenGameObjectsAtPosition(new Point(1, 0));
            var lastSeenWallAfterSeeingAgain = lastSeenGameObjectsAfterSeeingAgain.FirstOrDefault(o => o is Wall);

            Assert.IsNull(lastSeenWallAfterSeeingAgain);
        }

        [TestMethod]
        public void LastSeenEntity_Should_Not_Include_Environmental_Effect()
        {
            // Arrange
            // Ensure a rubble is always created
            _gameWorld.TestContextualEnhancedRandom.KnownSeries.Add(nameof(ExplodeTileCommand), new ShaiRandom.Generators.KnownSeriesRandom(boolSeries: [true]));

            var lines = new[]
            {
                ".#.",
                ".#.",
                "..."
            };

            var mapTemplate = new MapTemplate(lines);

            var mapGenerator = new SpecificMapGenerator(
                _gameWorld.GameObjectFactory,
                mapTemplate
                    .Where(m => m.Char == '#')
                    .Select(m => m.Point).ToList());

            NewGameWithTestLevelGenerator(
                _gameWorld,
                mapGenerator,
                playerPosition: new Point(0, 0),
                mapWidth: mapTemplate.Bounds.Width,
                mapHeight: mapTemplate.Bounds.Height);

            var commandFactory = Container.Resolve<ICommandCollection>();

            // Act and Assert

            var spawnEnvironmentalEffectParams = new SpawnEnvironmentalEffectParams()
                .WithEnvironmentalEffectType(EnvironmentalEffectType.MissileTargetType)
                .AtPosition(new Point(1, 0))
                .OnMap(_gameWorld.CurrentMap.Id);

            _gameWorld.SpawnEnvironmentalEffect(spawnEnvironmentalEffectParams);

            var environmentalEffect = spawnEnvironmentalEffectParams.Result;

            environmentalEffect.Duration = 3;

            _gameWorld.MoveRequest(Direction.Down);
            _gameWorld.MoveRequest(Direction.DownRight);

            var lastSeenGameObjects = _gameWorld.GetLastSeenGameObjectsAtPosition(new Point(1, 0));
            var lastSeenEnvironmentalEffect = lastSeenGameObjects.FirstOrDefault(o => o is EnvironmentalEffect);

            Assert.IsNull(lastSeenEnvironmentalEffect);
        }
    }
}
