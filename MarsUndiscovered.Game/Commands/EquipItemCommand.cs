using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Commands
{
    public class EquipItemCommand : BaseMarsGameActionCommand
    {
        public Item Item { get; set; }
        private Item _previousItem;
        private bool _isAlreadyEquipped;
        private bool _canEquipType => GameWorld.Inventory.CanTypeBeEquipped(Item);

        public EquipItemCommand(IGameWorld gameWorld) : base(gameWorld)
        {
            EndsPlayerTurn = true;
        }

        public void Initialise(Item item)
        {
            Item = item;
        }

        protected override CommandResult ExecuteInternal()
        {
            if (!_canEquipType)
                return Result(CommandResult.NoMove(this, "Cannot equip this type of item"));

            _previousItem = GameWorld.Inventory.GetEquippedItemOfType(Item.ItemType);

            var currentItemDescription = String.Empty;

            if (_previousItem != null)
                currentItemDescription = GameWorld.Inventory.ItemTypeDiscoveries.GetInventoryDescriptionAsSingleItemLowerCase(_previousItem);

            _isAlreadyEquipped = GameWorld.Inventory.IsEquipped(Item);

            if (_isAlreadyEquipped)
                return Result(CommandResult.NoMove(this, "Item is already equipped"));

            GameWorld.Inventory.Unequip(_previousItem);
            GameWorld.Inventory.Equip(Item);
            var itemDescription = GameWorld.Inventory.ItemTypeDiscoveries.GetInventoryDescriptionAsSingleItemLowerCase(Item);

            if (Item.ItemType is Weapon)
            {
                if (!string.IsNullOrEmpty(currentItemDescription))
                    currentItemDescription = $"; I sheathe {currentItemDescription}";

                return Result(CommandResult.Success(this, $"I wield {itemDescription}{currentItemDescription}"));
            }

            if (!string.IsNullOrEmpty(currentItemDescription))
                currentItemDescription = $"; I unequip {currentItemDescription}";

            return Result(CommandResult.Success(this, $"I equip {itemDescription}{currentItemDescription}"));
        }
    }
}
