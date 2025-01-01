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
    public class ApplyForcePushCommandTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        public void ApplyForcePushCommand_Should_Push_All_Actors_Away_From_Location()
        {
            // Arrange
            var lines = new[]
            {
                ".......",
                ".......",
                ".......",
                ".......",
                ".......",
                ".......",
                "......."
            };

            var mapTemplate = new MapTemplate(lines);
            
            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, mapTemplate.Where(m => m.Char == '#').Select(m => m.Point).ToList());

            NewGameWithTestLevelGenerator(
                _gameWorld,
                mapGenerator,
                playerPosition: new Point(3, 3),
                mapWidth: mapTemplate.Bounds.Width,
                mapHeight: mapTemplate.Bounds.Height);

            var spawnItemParams = new SpawnItemParams().WithItemType(ItemType.ForcePush).IntoPlayerInventory();
            _gameWorld.SpawnItem(spawnItemParams);

            var item = spawnItemParams.Result;

            var monster1Params = new SpawnMonsterParams()
                .AtPosition(new Point(3, 2))
                .WithBreed(Breed.Breeds.First().Value)
                .OnMap(_gameWorld.CurrentMap.Id);
            
            _gameWorld.SpawnMonster(monster1Params);

            var monster1 = monster1Params.Result;
            
            var monster2Params = new SpawnMonsterParams()
                .AtPosition(new Point(2, 3))
                .WithBreed(Breed.Breeds.First().Value)
                .OnMap(_gameWorld.CurrentMap.Id);
            
            _gameWorld.SpawnMonster(monster2Params);

            var monster2 = monster2Params.Result;
            
            var monster3Params = new SpawnMonsterParams()
                .AtPosition(new Point(3, 4))
                .WithBreed(Breed.Breeds.First().Value)
                .OnMap(_gameWorld.CurrentMap.Id);
            
            _gameWorld.SpawnMonster(monster3Params);

            var monster3 = monster3Params.Result;
            
            var monster4Params = new SpawnMonsterParams()
                .AtPosition(new Point(4, 3))
                .WithBreed(Breed.Breeds.First().Value)
                .OnMap(_gameWorld.CurrentMap.Id);
            
            _gameWorld.SpawnMonster(monster4Params);

            var monster4 = monster4Params.Result;
            
            var commandFactory = Container.Resolve<ICommandCollection>();

            var applyForcePushCommand = commandFactory.CreateCommand<ApplyForcePushCommand>(_gameWorld);
            applyForcePushCommand.Initialise(item, new Point(3, 3), 2, 1);
    
            // Act
            var result = applyForcePushCommand.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.Success, result.Result);
            
            Assert.AreEqual(new Point(3, 0), monster1.Position);
            Assert.AreEqual(new Point(0, 3), monster2.Position);
            Assert.AreEqual(new Point(3, 6), monster3.Position);
            Assert.AreEqual(new Point(6, 3), monster4.Position);
            
            Assert.AreEqual("A force field radiates out from me!", result.Messages[0]);
            
            Assert.IsFalse(result.Command.EndsPlayerTurn);
            Assert.IsFalse(result.Command.RequiresPlayerInput);
            Assert.IsFalse(result.Command.InterruptsMovement);
        }

        [TestMethod]
        public void ApplyForcePushCommand_Should_Push_All_Actors_Away_From_Location_Diagonals()
        {
            // Arrange
            var lines = new[]
            {
                ".......",
                ".......",
                ".......",
                ".......",
                ".......",
                ".......",
                "......."
            };

            var mapTemplate = new MapTemplate(lines);
            
            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, mapTemplate.Where(m => m.Char == '#').Select(m => m.Point).ToList());

            NewGameWithTestLevelGenerator(
                _gameWorld,
                mapGenerator,
                playerPosition: new Point(3, 3),
                mapWidth: mapTemplate.Bounds.Width,
                mapHeight: mapTemplate.Bounds.Height);

            var spawnItemParams = new SpawnItemParams().WithItemType(ItemType.ForcePush).IntoPlayerInventory();
            _gameWorld.SpawnItem(spawnItemParams);

            var item = spawnItemParams.Result;

            var monster1Params = new SpawnMonsterParams()
                .AtPosition(new Point(2, 2))
                .WithBreed(Breed.Breeds.First().Value)
                .OnMap(_gameWorld.CurrentMap.Id);
            
            _gameWorld.SpawnMonster(monster1Params);

            var monster1 = monster1Params.Result;
            
            var monster2Params = new SpawnMonsterParams()
                .AtPosition(new Point(4, 4))
                .WithBreed(Breed.Breeds.First().Value)
                .OnMap(_gameWorld.CurrentMap.Id);
            
            _gameWorld.SpawnMonster(monster2Params);

            var monster2 = monster2Params.Result;
            
            var monster3Params = new SpawnMonsterParams()
                .AtPosition(new Point(2, 4))
                .WithBreed(Breed.Breeds.First().Value)
                .OnMap(_gameWorld.CurrentMap.Id);
            
            _gameWorld.SpawnMonster(monster3Params);

            var monster3 = monster3Params.Result;
            
            var monster4Params = new SpawnMonsterParams()
                .AtPosition(new Point(4, 2))
                .WithBreed(Breed.Breeds.First().Value)
                .OnMap(_gameWorld.CurrentMap.Id);
            
            _gameWorld.SpawnMonster(monster4Params);

            var monster4 = monster4Params.Result;
            
            var commandFactory = Container.Resolve<ICommandCollection>();

            var applyForcePushCommand = commandFactory.CreateCommand<ApplyForcePushCommand>(_gameWorld);
            applyForcePushCommand.Initialise(item, new Point(3, 3), 2, 1);
    
            // Act
            var result = applyForcePushCommand.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.Success, result.Result);
            
            Assert.AreEqual(new Point(0, 0), monster1.Position);
            Assert.AreEqual(new Point(6, 6), monster2.Position);
            Assert.AreEqual(new Point(0, 6), monster3.Position);
            Assert.AreEqual(new Point(6, 0), monster4.Position);
            
            Assert.AreEqual("A force field radiates out from me!", result.Messages[0]);
            
            Assert.IsFalse(result.Command.EndsPlayerTurn);
            Assert.IsFalse(result.Command.RequiresPlayerInput);
            Assert.IsFalse(result.Command.InterruptsMovement);
        } 
        
        [TestMethod]
        public void ApplyForcePushCommand_Should_Not_Push_Actors_Not_In_Range()
        {
            // Arrange
            var lines = new[]
            {
                ".......",
                ".......",
                ".......",
                ".......",
                ".......",
                ".......",
                "......."
            };

            var mapTemplate = new MapTemplate(lines);
            
            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, mapTemplate.Where(m => m.Char == '#').Select(m => m.Point).ToList());

            NewGameWithTestLevelGenerator(
                _gameWorld,
                mapGenerator,
                playerPosition: new Point(3, 3),
                mapWidth: mapTemplate.Bounds.Width,
                mapHeight: mapTemplate.Bounds.Height);

            var spawnItemParams = new SpawnItemParams().WithItemType(ItemType.ForcePush).IntoPlayerInventory();
            _gameWorld.SpawnItem(spawnItemParams);

            var item = spawnItemParams.Result;

            var monster1Params = new SpawnMonsterParams()
                .AtPosition(new Point(3, 1))
                .WithBreed(Breed.Breeds.First().Value)
                .OnMap(_gameWorld.CurrentMap.Id);
            
            _gameWorld.SpawnMonster(monster1Params);

            var monster1 = monster1Params.Result;
            
            var commandFactory = Container.Resolve<ICommandCollection>();

            var applyForcePushCommand = commandFactory.CreateCommand<ApplyForcePushCommand>(_gameWorld);
            applyForcePushCommand.Initialise(item, new Point(3, 3), 2, 1);
    
            // Act
            var result = applyForcePushCommand.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.Success, result.Result);
            
            Assert.AreEqual(new Point(3, 1), monster1.Position);
            
            Assert.AreEqual("A force field radiates out from me!", result.Messages[0]);
            
            Assert.IsFalse(result.Command.EndsPlayerTurn);
            Assert.IsFalse(result.Command.RequiresPlayerInput);
            Assert.IsFalse(result.Command.InterruptsMovement);
        }
        
        [TestMethod]
        public void ApplyForcePushCommand_Should_Not_Push_Beyond_Map_Bounds()
        {
            // Arrange
            var lines = new[]
            {
                "...",
                "...",
                "..."
            };

            var mapTemplate = new MapTemplate(lines);
            
            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, mapTemplate.Where(m => m.Char == '#').Select(m => m.Point).ToList());

            NewGameWithTestLevelGenerator(
                _gameWorld,
                mapGenerator,
                playerPosition: new Point(1, 1),
                mapWidth: mapTemplate.Bounds.Width,
                mapHeight: mapTemplate.Bounds.Height);

            var spawnItemParams = new SpawnItemParams().WithItemType(ItemType.ForcePush).IntoPlayerInventory();
            _gameWorld.SpawnItem(spawnItemParams);

            var item = spawnItemParams.Result;

            var monster1Params = new SpawnMonsterParams()
                .AtPosition(new Point(0, 1))
                .WithBreed(Breed.Breeds.First().Value)
                .OnMap(_gameWorld.CurrentMap.Id);
            
            _gameWorld.SpawnMonster(monster1Params);

            var monster1 = monster1Params.Result;
            
            var monster2Params = new SpawnMonsterParams()
                .AtPosition(new Point(2, 1))
                .WithBreed(Breed.Breeds.First().Value)
                .OnMap(_gameWorld.CurrentMap.Id);
            
            _gameWorld.SpawnMonster(monster2Params);

            var monster2 = monster2Params.Result;
            
            var monster3Params = new SpawnMonsterParams()
                .AtPosition(new Point(1, 0))
                .WithBreed(Breed.Breeds.First().Value)
                .OnMap(_gameWorld.CurrentMap.Id);
            
            _gameWorld.SpawnMonster(monster3Params);

            var monster3 = monster3Params.Result;
            
            var monster4Params = new SpawnMonsterParams()
                .AtPosition(new Point(1, 2))
                .WithBreed(Breed.Breeds.First().Value)
                .OnMap(_gameWorld.CurrentMap.Id);
            
            _gameWorld.SpawnMonster(monster4Params);

            var monster4 = monster4Params.Result;
            
            var commandFactory = Container.Resolve<ICommandCollection>();

            var applyForcePushCommand = commandFactory.CreateCommand<ApplyForcePushCommand>(_gameWorld);
            applyForcePushCommand.Initialise(item, new Point(1, 1), 2, 1);
    
            // Act
            var result = applyForcePushCommand.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.Success, result.Result);
            
            Assert.AreEqual(new Point(0, 1), monster1.Position);
            Assert.AreEqual(new Point(2, 1), monster2.Position);
            Assert.AreEqual(new Point(1, 0), monster3.Position);
            Assert.AreEqual(new Point(1, 2), monster4.Position);
            
            Assert.AreEqual("A force field radiates out from me!", result.Messages[0]);
            
            Assert.IsFalse(result.Command.EndsPlayerTurn);
            Assert.IsFalse(result.Command.RequiresPlayerInput);
            Assert.IsFalse(result.Command.InterruptsMovement);
        }
        
        [TestMethod]
        public void ApplyForcePushCommand_Should_Not_Push_Past_Walls()
        {
            // Arrange
            var lines = new[]
            {
                "...#...",
                ".......",
                ".......",
                ".......",
                ".......",
                ".......",
                "......."
            };

            var mapTemplate = new MapTemplate(lines);
            
            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, mapTemplate.Where(m => m.Char == '#').Select(m => m.Point).ToList());

            NewGameWithTestLevelGenerator(
                _gameWorld,
                mapGenerator,
                playerPosition: new Point(3, 3),
                mapWidth: mapTemplate.Bounds.Width,
                mapHeight: mapTemplate.Bounds.Height);

            var spawnItemParams = new SpawnItemParams().WithItemType(ItemType.ForcePush).IntoPlayerInventory();
            _gameWorld.SpawnItem(spawnItemParams);

            var item = spawnItemParams.Result;

            var monster1Params = new SpawnMonsterParams()
                .AtPosition(new Point(3, 2))
                .WithBreed(Breed.Breeds.First().Value)
                .OnMap(_gameWorld.CurrentMap.Id);
            
            _gameWorld.SpawnMonster(monster1Params);

            var monster1 = monster1Params.Result;
            
            var commandFactory = Container.Resolve<ICommandCollection>();

            var applyForcePushCommand = commandFactory.CreateCommand<ApplyForcePushCommand>(_gameWorld);
            applyForcePushCommand.Initialise(item, new Point(3, 3), 2, 1);
    
            // Act
            var result = applyForcePushCommand.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.Success, result.Result);
            
            Assert.AreEqual(new Point(3, 1), monster1.Position);
           
            Assert.AreEqual("A force field radiates out from me!", result.Messages[0]);
            
            Assert.IsFalse(result.Command.EndsPlayerTurn);
            Assert.IsFalse(result.Command.RequiresPlayerInput);
            Assert.IsFalse(result.Command.InterruptsMovement);
        }
        
        [TestMethod]
        public void ApplyForcePushCommand_Should_Not_Push_Past_Walls_Already_Adjacent()
        {
            // Arrange
            var lines = new[]
            {
                ".......",
                "...#...",
                ".......",
                ".......",
                ".......",
                ".......",
                "......."
            };

            var mapTemplate = new MapTemplate(lines);
            
            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, mapTemplate.Where(m => m.Char == '#').Select(m => m.Point).ToList());

            NewGameWithTestLevelGenerator(
                _gameWorld,
                mapGenerator,
                playerPosition: new Point(3, 3),
                mapWidth: mapTemplate.Bounds.Width,
                mapHeight: mapTemplate.Bounds.Height);

            var spawnItemParams = new SpawnItemParams().WithItemType(ItemType.ForcePush).IntoPlayerInventory();
            _gameWorld.SpawnItem(spawnItemParams);

            var item = spawnItemParams.Result;

            var monster1Params = new SpawnMonsterParams()
                .AtPosition(new Point(3, 2))
                .WithBreed(Breed.Breeds.First().Value)
                .OnMap(_gameWorld.CurrentMap.Id);
            
            _gameWorld.SpawnMonster(monster1Params);

            var monster1 = monster1Params.Result;
            
            var commandFactory = Container.Resolve<ICommandCollection>();

            var applyForcePushCommand = commandFactory.CreateCommand<ApplyForcePushCommand>(_gameWorld);
            applyForcePushCommand.Initialise(item, new Point(3, 3), 2, 2);
    
            // Act
            var result = applyForcePushCommand.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.Success, result.Result);
            
            Assert.AreEqual(new Point(3, 2), monster1.Position);
           
            Assert.AreEqual("A force field radiates out from me!", result.Messages[0]);
            
            Assert.IsFalse(result.Command.EndsPlayerTurn);
            Assert.IsFalse(result.Command.RequiresPlayerInput);
            Assert.IsFalse(result.Command.InterruptsMovement);
        }
        
        [TestMethod]
        public void ApplyForcePushCommand_Should_Push_Actors_In_A_Line()
        {
            // Arrange
            var lines = new[]
            {
                ".......",
                ".......",
                ".......",
                ".......",
                ".......",
                ".......",
                ".......",
                ".......",
                ".......",
                "......."
            };

            var mapTemplate = new MapTemplate(lines);
            
            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, mapTemplate.Where(m => m.Char == '#').Select(m => m.Point).ToList());

            NewGameWithTestLevelGenerator(
                _gameWorld,
                mapGenerator,
                playerPosition: new Point(4, 8),
                mapWidth: mapTemplate.Bounds.Width,
                mapHeight: mapTemplate.Bounds.Height);

            var spawnItemParams = new SpawnItemParams().WithItemType(ItemType.ForcePush).IntoPlayerInventory();
            _gameWorld.SpawnItem(spawnItemParams);

            var item = spawnItemParams.Result;

            var monster1Params = new SpawnMonsterParams()
                .AtPosition(new Point(4, 7))
                .WithBreed(Breed.Breeds.First().Value)
                .OnMap(_gameWorld.CurrentMap.Id);
            
            _gameWorld.SpawnMonster(monster1Params);

            var monster1 = monster1Params.Result;
            
            var monster2Params = new SpawnMonsterParams()
                .AtPosition(new Point(4, 6))
                .WithBreed(Breed.Breeds.First().Value)
                .OnMap(_gameWorld.CurrentMap.Id);
            
            _gameWorld.SpawnMonster(monster2Params);

            var monster2 = monster2Params.Result;
            
            var monster3Params = new SpawnMonsterParams()
                .AtPosition(new Point(4, 4))
                .WithBreed(Breed.Breeds.First().Value)
                .OnMap(_gameWorld.CurrentMap.Id);
            
            _gameWorld.SpawnMonster(monster3Params);

            var monster3 = monster3Params.Result;
            
            var commandFactory = Container.Resolve<ICommandCollection>();

            var applyForcePushCommand = commandFactory.CreateCommand<ApplyForcePushCommand>(_gameWorld);
            applyForcePushCommand.Initialise(item, new Point(4, 8), 3, 99);
    
            // Act
            var result = applyForcePushCommand.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.Success, result.Result);
            
            Assert.AreEqual(new Point(4, 4), monster1.Position);
            Assert.AreEqual(new Point(4, 3), monster2.Position);
            Assert.AreEqual(new Point(4, 1), monster3.Position);
            
            Assert.AreEqual("A force field radiates out from me!", result.Messages[0]);
            
            Assert.IsFalse(result.Command.EndsPlayerTurn);
            Assert.IsFalse(result.Command.RequiresPlayerInput);
            Assert.IsFalse(result.Command.InterruptsMovement);
        }
        
        [TestMethod]
        public void ApplyForcePushCommand_Should_Push_All_Actors_Away_From_Location_With_Blocking_Wall()
        {
            // Arrange
            var lines = new[]
            {
                ".......",
                ".#.....",
                ".......",
            };

            var mapTemplate = new MapTemplate(lines);
            
            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, mapTemplate.Where(m => m.Char == '#').Select(m => m.Point).ToList());

            NewGameWithTestLevelGenerator(
                _gameWorld,
                mapGenerator,
                playerPosition: new Point(4, 1),
                mapWidth: mapTemplate.Bounds.Width,
                mapHeight: mapTemplate.Bounds.Height);

            var spawnItemParams = new SpawnItemParams().WithItemType(ItemType.ForcePush).IntoPlayerInventory();
            _gameWorld.SpawnItem(spawnItemParams);

            var item = spawnItemParams.Result;

            var monster1Params = new SpawnMonsterParams()
                .AtPosition(new Point(3, 1))
                .WithBreed(Breed.Breeds.First().Value)
                .OnMap(_gameWorld.CurrentMap.Id);
            
            _gameWorld.SpawnMonster(monster1Params);

            var monster1 = monster1Params.Result;
            
            var commandFactory = Container.Resolve<ICommandCollection>();

            var applyForcePushCommand = commandFactory.CreateCommand<ApplyForcePushCommand>(_gameWorld);
            applyForcePushCommand.Initialise(item, new Point(4, 1), 2, 1);
    
            // Act
            var result = applyForcePushCommand.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.Success, result.Result);
            
            Assert.AreEqual(new Point(2, 1), monster1.Position);
            
            Assert.AreEqual("A force field radiates out from me!", result.Messages[0]);
            
            Assert.IsFalse(result.Command.EndsPlayerTurn);
            Assert.IsFalse(result.Command.RequiresPlayerInput);
            Assert.IsFalse(result.Command.InterruptsMovement);
        }
    }
}
