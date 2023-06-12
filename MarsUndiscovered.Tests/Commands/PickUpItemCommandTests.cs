using FrigidRogue.MonoGame.Core.Components;
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
            NewGameWithCustomMapNoMonstersNoItems(_gameWorld);
            _gameWorld.Player.Position = new Point(0, 0);
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe).AtPosition(_gameWorld.Player.Position));
            var item = _gameWorld.Items.First().Value;

            var commandFactory = Container.Resolve<ICommandFactory>();

            var pickUpItemCommand = commandFactory.CreatePickUpItemCommand(_gameWorld);
            pickUpItemCommand.Initialise(item, _gameWorld.Player);

            // Act
            var result = pickUpItemCommand.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.Success, result.Result);
            Assert.IsFalse(_gameWorld.CurrentMap.GetObjectsAt(_gameWorld.Player.Position).Any(m => m is Item));
            Assert.AreEqual("You pick up a Magnesium Pipe", result.Messages[0]);
        }

        [TestMethod]
        public void PickUpItemCommand_On_Item_Should_Not_Pick_Up_Item_If_Inventory_Is_Full()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems(_gameWorld);
            _gameWorld.Player.Position = new Point(0, 0);

            var i = 0;
            while(_gameWorld.Inventory.CanPickUpItem)
            {
                _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe));
                _gameWorld.Inventory.Add(_gameWorld.Items.Values.ToList()[i]);
                i++;
            }

            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe).AtPosition(_gameWorld.Player.Position));

            var item = _gameWorld.Items.Values.ToList()[i];

            var commandFactory = Container.Resolve<ICommandFactory>();

            var pickUpItemCommand = commandFactory.CreatePickUpItemCommand(_gameWorld);
            pickUpItemCommand.Initialise(item, _gameWorld.Player);

            // Act
            var result = pickUpItemCommand.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.NoMove, result.Result);
            Assert.IsTrue(_gameWorld.CurrentMap.GetObjectsAt(_gameWorld.Player.Position).Any(m => m is Item));
            Assert.AreEqual("Your inventory is too full to pick up a Magnesium Pipe", result.Messages[0]);
        }

        [TestMethod]
        public void PickUpItemCommand_On_Item_Should_Not_Pick_Up_Item_If_Not_Player()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems(_gameWorld);
            _gameWorld.Player.Position = new Point(0, 1);

            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(new Point(0, 0)));
            var monster = _gameWorld.Monsters.Values.First();

            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe).AtPosition(monster.Position));
            var item = _gameWorld.Items.First().Value;
            var commandFactory = Container.Resolve<ICommandFactory>();

            var pickUpItemCommand = commandFactory.CreatePickUpItemCommand(_gameWorld);
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
