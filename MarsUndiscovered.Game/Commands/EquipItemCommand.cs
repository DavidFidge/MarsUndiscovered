﻿using FrigidRogue.MonoGame.Core.Components;

using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Commands
{
    public class EquipItemCommand : BaseMarsGameActionCommand<EquipItemSaveData>
    {
        public Item Item => GameWorld.Items[_data.ItemId];
        private Item _previousItem => _data.PreviousItemId == null ? null : GameWorld.Items[_data.PreviousItemId.Value];
        private bool _isAlreadyEquipped => _data.IsAlreadyEquipped;
        private bool _canEquipType => GameWorld.Inventory.CanTypeBeEquipped(Item);

        public EquipItemCommand(IGameWorld gameWorld) : base(gameWorld)
        {
            EndsPlayerTurn = true;
            PersistForReplay = true;
        }

        public void Initialise(Item item)
        {
            _data.ItemId = item.ID;
        }

        protected override CommandResult ExecuteInternal()
        {
            if (!_canEquipType)
                return Result(CommandResult.NoMove(this, $"Cannot equip this type of item"));

            var previousItem = GameWorld.Inventory.GetEquippedItemOfType(Item.ItemType);
            _data.PreviousItemId = previousItem?.ID;

            var currentItemDescription = String.Empty;

            if (previousItem != null)
                currentItemDescription = GameWorld.Inventory.ItemTypeDiscoveries.GetInventoryDescriptionAsSingleItemLowerCase(_previousItem);

            _data.IsAlreadyEquipped = GameWorld.Inventory.IsEquipped(Item);

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
            
            if (_data.PreviousItemId != null)
                GameWorld.Inventory.Equip(_previousItem);
        }
    }
}
