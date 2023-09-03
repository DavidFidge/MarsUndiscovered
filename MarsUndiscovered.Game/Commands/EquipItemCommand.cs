using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Commands
{
    public class EquipItemCommand : BaseMarsGameActionCommand<EquipItemSaveData>
    {
        public Item Item { get; private set; }
        private Item _previousItem;
        private bool _isAlreadyEquipped;
        private bool _canEquipType;

        public EquipItemCommand(IGameWorld gameWorld) : base(gameWorld)
        {
            EndsPlayerTurn = true;
            PersistForReplay = true;
        }

        public void Initialise(Item item)
        {
            Item = item;
        }

        public override IMemento<EquipItemSaveData> GetSaveState()
        {
            var memento = new Memento<EquipItemSaveData>(new EquipItemSaveData());
            base.PopulateSaveState(memento.State);
            memento.State.ItemId = Item.ID;
            return memento;
        }

        public override void SetLoadState(IMemento<EquipItemSaveData> memento)
        {
            base.PopulateLoadState(memento.State);
            Item = GameWorld.Items[memento.State.ItemId];
        }

        protected override CommandResult ExecuteInternal()
        {
            _canEquipType = GameWorld.Inventory.CanTypeBeEquipped(Item);

            if (!_canEquipType)
                return Result(CommandResult.NoMove(this, $"Cannot equip this type of item"));

            _previousItem = GameWorld.Inventory.GetEquippedItemOfType(Item.ItemType);

            var currentItemDescription = String.Empty;

            if (_previousItem != null)
                currentItemDescription = GameWorld.Inventory.ItemTypeDiscoveries.GetInventoryDescriptionAsSingleItemLowerCase(_previousItem);

            _isAlreadyEquipped = GameWorld.Inventory.IsEquipped(Item);

            if (_isAlreadyEquipped)
                return Result(CommandResult.NoMove(this, $"Item is already equipped"));

            GameWorld.Inventory.Unequip(_previousItem);
            GameWorld.Inventory.Equip(Item);
            var itemDescription = GameWorld.Inventory.ItemTypeDiscoveries.GetInventoryDescriptionAsSingleItemLowerCase(Item);

            if (Item.ItemType is Weapon)
            {
                if (!string.IsNullOrEmpty(currentItemDescription))
                    currentItemDescription = $"; you sheathe {currentItemDescription}";

                return Result(CommandResult.Success(this, $"You wield {itemDescription}{currentItemDescription}"));
            }

            if (!string.IsNullOrEmpty(currentItemDescription))
                currentItemDescription = $"; you unequip {currentItemDescription}";

            GameWorld.Player.RecalculateAttacks();

            return Result(CommandResult.Success(this, $"You equip {itemDescription}{currentItemDescription}"));
        }

        protected override void UndoInternal()
        {
            if (_isAlreadyEquipped || !_canEquipType)
                return;

            GameWorld.Inventory.Unequip(Item);
            GameWorld.Inventory.Equip(_previousItem);
        }
    }
}
