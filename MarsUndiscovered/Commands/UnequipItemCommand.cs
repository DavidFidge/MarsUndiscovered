using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using MarsUndiscovered.Components;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Commands
{
    public class UnequipItemCommand : BaseMarsGameActionCommand<UnequipItemSaveData>
    {
        private bool _isNotEquipped;
        public Item Item { get; private set; }

        public UnequipItemCommand(IGameWorld gameWorld) : base(gameWorld)
        {
        }

        public void Initialise(Item item)
        {
            Item = item;
        }

        public override IMemento<UnequipItemSaveData> GetSaveState()
        {
            var memento = new Memento<UnequipItemSaveData>(new UnequipItemSaveData());
            base.PopulateSaveState(memento.State);
            memento.State.ItemId = Item.ID;
            return memento;
        }

        public override void SetLoadState(IMemento<UnequipItemSaveData> memento)
        {
            base.PopulateLoadState(memento.State);
            Item = GameWorld.Items[memento.State.ItemId];
        }

        protected override CommandResult ExecuteInternal()
        {
            var itemDescription = GameWorld.Inventory.ItemTypeDiscoveries.GetInventoryDescriptionAsSingleItemLowerCase(Item);

            _isNotEquipped = !GameWorld.Inventory.IsEquipped(Item);
            if (_isNotEquipped)
                return Result(CommandResult.NoMove(this, $"Item is already unequipped"));

            GameWorld.Inventory.Unequip(Item);

            if (Item.ItemType is Weapon)
            {
                return Result(CommandResult.Success(this, $"You sheathe {itemDescription}"));
            }

            return Result(CommandResult.Success(this, $"You unequip {itemDescription}"));
        }

        protected override void UndoInternal()
        {
            if (!_isNotEquipped)
                GameWorld.Inventory.Equip(Item);
        }
    }
}
