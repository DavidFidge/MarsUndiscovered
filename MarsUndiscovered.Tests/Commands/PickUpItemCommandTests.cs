using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Tests.Components;
using SadRogue.Primitives;

namespace MarsUndiscovered.Tests.Commands
{
    [TestClass]
    public class PickUpItemCommandTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        public void PickUpItemCommand_On_Item_Should_Pick_Up_Item()
        {
            // Arrange
            NewGameWithTestLevelGenerator(_gameWorld);
            var spawnItemParams = new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe);
            _gameWorld.SpawnItem(spawnItemParams);
            
            // Items always try and find a free floor spot so cannot be spawned directly on top of
            // the player via .AtPosition().  So instead move the player onto the item
            _gameWorld.Player.Position = spawnItemParams.Result.Position;
           
            var item = _gameWorld.Items.First().Value;

            var commandFactory = Container.Resolve<ICommandCollection>();

            var pickUpItemCommand = commandFactory.CreateCommand<PickUpItemCommand>(_gameWorld);
            pickUpItemCommand.Initialise(item, _gameWorld.Player);

            // Act
            var result = pickUpItemCommand.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.Success, result.Result);
            Assert.IsFalse(_gameWorld.CurrentMap.GetObjectsAt(_gameWorld.Player.Position).Any(m => m is Item));
            Assert.AreEqual("I pick up a Magnesium Pipe", result.Messages[0]);
            
            Assert.IsFalse(result.Command.EndsPlayerTurn);
            Assert.IsFalse(result.Command.RequiresPlayerInput);
            Assert.IsFalse(result.Command.InterruptsMovement);
        }

        [TestMethod]
        public void PickUpItemCommand_On_Item_Should_Not_Pick_Up_Item_If_Inventory_Is_Full()
        {
            // Arrange
            NewGameWithTestLevelGenerator(_gameWorld, playerPosition: new Point(0, 0));

            var i = 0;
            while(_gameWorld.Inventory.CanPickUpItem)
            {
                _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe));
                _gameWorld.Inventory.Add(_gameWorld.Items.Values.ToList()[i]);
                i++;
            }

            var spawnItemParams = new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe);
            _gameWorld.SpawnItem(spawnItemParams);
            var item = spawnItemParams.Result;

            _gameWorld.Player.Position = item.Position;

            var commandFactory = Container.Resolve<ICommandCollection>();

            var pickUpItemCommand = commandFactory.CreateCommand<PickUpItemCommand>(_gameWorld);
            pickUpItemCommand.Initialise(item, _gameWorld.Player);

            // Act
            var result = pickUpItemCommand.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.NoMove, result.Result);
            Assert.IsTrue(_gameWorld.CurrentMap.GetObjectsAt(_gameWorld.Player.Position).Any(m => m is Item));
            Assert.AreEqual("My inventory is too full to pick up a Magnesium Pipe", result.Messages[0]);
        }

        [TestMethod]
        public void PickUpItemCommand_On_Item_Should_Not_Pick_Up_Item_If_Not_Player()
        {
            // Arrange
            NewGameWithTestLevelGenerator(_gameWorld, playerPosition: new Point(0, 1));

            var spawnMonsterParams = new SpawnMonsterParams().WithBreed("Roach");
            _gameWorld.SpawnMonster(spawnMonsterParams);
            var monster = spawnMonsterParams.Result;

            var spawnItemParams = new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe);
            _gameWorld.SpawnItem(spawnItemParams);
            var item = spawnItemParams.Result;
            monster.Position = item.Position;
            
            var commandFactory = Container.Resolve<ICommandCollection>();

            var pickUpItemCommand = commandFactory.CreateCommand<PickUpItemCommand>(_gameWorld);
            pickUpItemCommand.Initialise(item, monster);

            // Act
            var result = pickUpItemCommand.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.Exception, result.Result);
            Assert.IsTrue(_gameWorld.CurrentMap.GetObjectsAt(monster.Position).Any(m => m is Item));
            Assert.AreEqual("Monsters currently do not pick up items", result.Messages[0]);
        }
    }
}
