using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.Maps;
using MarsUndiscovered.Tests.Components;
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
    }
}
