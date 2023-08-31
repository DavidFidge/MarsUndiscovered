using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using GoRogue.GameFramework;

using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Game.Commands
{
    public class ApplyItemCommand : BaseMarsGameActionCommand<ApplyItemCommandSaveData>
    {
        public Item Item { get; private set; }
        public IGameObject GameObject { get; private set; }
        private bool _canApply;
        private bool _isItemTypeDiscovered;
        private Keys _itemKey;
        private bool _isCharged;

        public ApplyItemCommand(IGameWorld gameWorld) : base(gameWorld)
        {
        }

        public void Initialise(IGameObject gameObject, Item item)
        {
            GameObject = gameObject;
            Item = item;
        }

        public override IMemento<ApplyItemCommandSaveData> GetSaveState()
        {
            var memento = new Memento<ApplyItemCommandSaveData>(new ApplyItemCommandSaveData());
            base.PopulateSaveState(memento.State);
            memento.State.GameObjectId = GameObject.ID;
            memento.State.ItemId = Item.ID;
            memento.State.CanApply = _canApply;
            memento.State.IsItemTypeDiscovered = _isItemTypeDiscovered;
            memento.State.ItemKey = _itemKey;
            memento.State.IsCharged = _isCharged;
            
            return memento;
        }

        public override void SetLoadState(IMemento<ApplyItemCommandSaveData> memento)
        {
            base.PopulateLoadState(memento.State);
            GameObject = GameWorld.GameObjects[memento.State.GameObjectId];
            Item = GameWorld.Items[memento.State.ItemId];
            _canApply = memento.State.CanApply;
            _isItemTypeDiscovered = memento.State.IsItemTypeDiscovered;
            _itemKey = memento.State.ItemKey;
            _isCharged = memento.State.IsCharged;
        }

        protected override CommandResult ExecuteInternal()
        {
            _canApply = GameWorld.Inventory.CanTypeBeApplied(Item);

            if (!_canApply)
                return Result(CommandResult.NoMove(this, $"{GameWorld.Inventory.ItemTypeDiscoveries.GetInventoryDescriptionAsSingleItem(Item)} cannot be applied. Did you mean to equip it?"));

            _isCharged = Item.ItemType is NanoFlask || (Item.ItemType is Gadget && Item.CurrentRechargeDelay > 0);
            
            _itemKey = GameWorld.Inventory.GetKeyForItem(Item);
            _isItemTypeDiscovered = GameWorld.Inventory.ItemTypeDiscoveries.IsItemTypeDiscovered(Item);

            if (Item.ItemType is Gadget && Item.CurrentRechargeDelay > 0)
            {
                var itemDescription = GameWorld.Inventory.ItemTypeDiscoveries.GetInventoryDescriptionWithoutPrefix(Item);
                
                return Result(CommandResult.NoMove(this,
                    $"Cannot apply the {itemDescription}. It is still recharging."));
            }

            string message;

            if (!_isItemTypeDiscovered)
            {
                GameWorld.Inventory.ItemTypeDiscoveries.SetItemTypeDiscovered(Item);
                
                message =
                    $"The {GameWorld.Inventory.ItemTypeDiscoveries.GetUndiscoveredDescription(Item)} is {GameWorld.Inventory.ItemTypeDiscoveries.GetInventoryDescriptionAsSingleItemLowerCase(Item)}!";
            }
            else
            {
                message = $"You apply {GameWorld.Inventory.ItemTypeDiscoveries.GetInventoryDescriptionAsSingleItemLowerCase(Item)}";
            }

            Item.ResetRechargeDelay();

            if (Item.ItemType is NanoFlask nanoflask)
            {
                if (nanoflask.RemoveFromInventoryOnApply)
                    GameWorld.Inventory.Remove(Item);
            }

            BaseGameActionCommand subsequentCommand;

            switch (Item.ItemType)
            {
                case ShieldGenerator:
                {
                    var command = CommandFactory.CreateApplyShieldCommand(GameWorld);
                    command.Initialise(Item, GameWorld.Player);
                    subsequentCommand = command;
                    break;
                }
                case HealingBots:
                {
                    var command = CommandFactory.CreateApplyHealingBotsCommand(GameWorld);
                    command.Initialise(Item, GameWorld.Player);
                    subsequentCommand = command;
                    break;
                }
                default:
                    throw new Exception($"Apply for ItemType {Item.ItemType} has not been implemented");
            }
            
            return Result(CommandResult.Success(this, message, subsequentCommand));
        }

        protected override void UndoInternal()
        {
            if (!_canApply || !_isCharged)
                return;
            
            if (!_isItemTypeDiscovered)
                GameWorld.Inventory.ItemTypeDiscoveries.SetItemTypeUndiscovered(Item);
            
            GameWorld.Inventory.Add(Item, _itemKey);
            Item.CurrentRechargeDelay = 0;
        }
    }
}
