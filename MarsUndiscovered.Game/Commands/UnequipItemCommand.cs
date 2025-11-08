using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Commands
{
    public class UnequipItemCommand : BaseMarsGameActionCommand
    {
        public Item Item { get; set; }

        public UnequipItemCommand(IGameWorld gameWorld) : base(gameWorld)
        {
            EndsPlayerTurn = true;
        }

        public void Initialise(Item item)
        {
            Item = item;
        }

        protected override CommandResult ExecuteInternal()
        {
            var itemDescription = GameWorld.Inventory.ItemTypeDiscoveries.GetInventoryDescriptionAsSingleItemLowerCase(Item);
            
            var isNotEquipped = !GameWorld.Inventory.IsEquipped(Item);
            
            if (isNotEquipped)
                return Result(CommandResult.NoMove(this, "Item is already unequipped"));

            GameWorld.Inventory.Unequip(Item);

            if (Item.ItemType is Weapon)
            {
                return Result(CommandResult.Success(this, $"I sheathe {itemDescription}"));
            }

            return Result(CommandResult.Success(this, $"I unequip {itemDescription}"));
        }
    }
}
