using System;
using System.Collections.Generic;

using AutoMapper;

using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using MarsUndiscovered.Components;
using MarsUndiscovered.Interfaces;

using Microsoft.Xna.Framework;

namespace MarsUndiscovered.Commands
{
    public class EquipItemCommand : BaseMarsGameActionCommand<EquipItemSaveData>
    {
        public Item Item { get; private set; }
        private Item _previousItem;
        private bool _isAlreadyEquipped;

        public EquipItemCommand(IGameWorld gameWorld) : base(gameWorld)
        {
        }

        public void Initialise(Item item)
        {
            Item = item;
        }

        public override IMemento<EquipItemSaveData> GetSaveState(IMapper mapper)
        {
            return Memento<EquipItemSaveData>.CreateWithAutoMapper(this, mapper);
        }

        public override void SetLoadState(IMemento<EquipItemSaveData> memento, IMapper mapper)
        {
            base.SetLoadState(memento, mapper);

            Memento<EquipItemSaveData>.SetWithAutoMapper(this, memento, mapper);
            Item = GameWorld.Items[memento.State.ItemId];
        }

        protected override CommandResult ExecuteInternal()
        {
            var itemDescription = GameWorld.Inventory.GetInventoryDescriptionAsSingleItemLowerCase(Item);
            _previousItem = GameWorld.Inventory.GetEquippedItemOfType(Item.ItemType);

            var currentItemDescription = String.Empty;

            if (_previousItem != null)
                currentItemDescription = GameWorld.Inventory.GetInventoryDescriptionAsSingleItemLowerCase(_previousItem);

            _isAlreadyEquipped = GameWorld.Inventory.IsEquipped(Item);

            if (_isAlreadyEquipped)
                return Result(CommandResult.Success(this, $"Item is already equipped"));

            GameWorld.Inventory.Unequip(_previousItem);
            GameWorld.Inventory.Equip(Item);

            if (Item.ItemType is Weapon)
            {
                if (!string.IsNullOrEmpty(currentItemDescription))
                    currentItemDescription = $"; you sheathe {currentItemDescription}";

                return Result(CommandResult.Success(this, $"You wield {itemDescription}{currentItemDescription}"));
            }

            if (!string.IsNullOrEmpty(currentItemDescription))
                currentItemDescription = $"; you unequip {currentItemDescription}";

            return Result(CommandResult.Success(this, $"You equip {itemDescription}{currentItemDescription}"));
        }

        protected override void UndoInternal()
        {
            if (_isAlreadyEquipped)
                return;

            GameWorld.Inventory.Unequip(Item);
            GameWorld.Inventory.Equip(_previousItem);
        }
    }
}
