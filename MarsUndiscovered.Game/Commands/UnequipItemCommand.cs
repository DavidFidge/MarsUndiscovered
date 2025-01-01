using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Commands
{
    public class UnequipItemCommand : BaseMarsGameActionCommand<UnequipItemSaveData>
    {
        public Item Item => GameWorld.Items[_data.ItemId];

        public UnequipItemCommand(IGameWorld gameWorld) : base(gameWorld)
        {
            EndsPlayerTurn = true;
        }

        public void Initialise(Item item)
        {
            _data.ItemId = item.ID;
        }

        protected override CommandResult ExecuteInternal()
        {
            var itemDescription = GameWorld.Inventory.ItemTypeDiscoveries.GetInventoryDescriptionAsSingleItemLowerCase(Item);
            
            _data.IsNotEquipped = !GameWorld.Inventory.IsEquipped(Item);
            
            if (_data.IsNotEquipped)
                return Result(CommandResult.NoMove(this, "Item is already unequipped"));

            GameWorld.Inventory.Unequip(Item);

            if (Item.ItemType is Weapon)
            {
                return Result(CommandResult.Success(this, $"I sheathe {itemDescription}"));
            }

            return Result(CommandResult.Success(this, $"I unequip {itemDescription}"));
        }

        protected override void UndoInternal()
        {
            if (!_data.IsNotEquipped)
                GameWorld.Inventory.Equip(Item);
        }
    }
}
