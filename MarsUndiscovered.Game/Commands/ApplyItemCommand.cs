using FrigidRogue.MonoGame.Core.Components;
using GoRogue.GameFramework;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Commands
{
    public class ApplyItemCommand : BaseMarsGameActionCommand
    {
        public Item Item { get; set; }
        public IGameObject GameObject { get; set; }

        public ApplyItemCommand(IGameWorld gameWorld) : base(gameWorld)
        {
            EndsPlayerTurn = true;
        }

        public void Initialise(IGameObject gameObject, Item item)
        {
            GameObject = gameObject;
            Item = item;
        }

        protected override CommandResult ExecuteInternal()
        {
            var canApply = GameWorld.Inventory.CanTypeBeApplied(Item);

            if (!canApply)
                return Result(CommandResult.NoMove(this, $"{GameWorld.Inventory.ItemTypeDiscoveries.GetInventoryDescriptionAsSingleItem(Item)} cannot be applied. Did you mean to equip it?"));

            var isItemTypeDiscovered = GameWorld.Inventory.ItemTypeDiscoveries.IsItemTypeDiscovered(Item);

            if (Item.ItemType is Gadget && Item.CurrentRechargeDelay > 0)
            {
                var itemDescription = GameWorld.Inventory.ItemTypeDiscoveries.GetInventoryDescriptionWithoutPrefix(Item);
                
                return Result(CommandResult.NoMove(this,
                    $"Cannot apply the {itemDescription}. It is still recharging."));
            }

            string message;
            
            // Gadgets also get their enchant level identified since the effects
            // themselves reveal it
            if (Item.ItemType is Gadget)
            {
                Item.ItemDiscovery.IsEnchantLevelDiscovered = true;
            }
            
            if (!isItemTypeDiscovered)
            {
                GameWorld.Inventory.ItemTypeDiscoveries.SetItemTypeDiscovered(Item);
                
                message =
                    $"The {GameWorld.Inventory.ItemTypeDiscoveries.GetUndiscoveredDescription(Item)} is {GameWorld.Inventory.ItemTypeDiscoveries.GetInventoryDescriptionAsSingleItemLowerCase(Item)}!";
            }
            else
            {
                message = $"I apply {GameWorld.Inventory.ItemTypeDiscoveries.GetInventoryDescriptionAsSingleItemLowerCase(Item)}";
            }
            
            Item.ResetRechargeDelay();

            if (Item.ItemType is NanoFlask)
            {
                GameWorld.Inventory.Remove(Item);
            }

            BaseGameActionCommand subsequentCommand = null;

            switch (Item.ItemType)
            {
                case ShieldGenerator:
                {
                    var command = GameWorld.CommandCollection.CreateCommand<ApplyShieldCommand>(GameWorld);
                    command.Initialise(Item, GameWorld.Player);
                    subsequentCommand = command;
                    break;
                }
                case ForcePush:
                {
                    var command = GameWorld.CommandCollection.CreateCommand<ApplyForcePushCommand>(GameWorld);
                    command.Initialise(Item, GameWorld.GetPlayerPosition(), Item.PushPullDistance, Item.PushPullRadius);
                    subsequentCommand = command;
                    break;
                }
                case HealingBots:
                {
                    var command = GameWorld.CommandCollection.CreateCommand<ApplyHealingBotsCommand>(GameWorld);
                    command.Initialise(Item, GameWorld.Player);
                    subsequentCommand = command;
                    break;
                }
                case EnhancementBots:
                {
                    RequiresPlayerInput = true;
                    EndsPlayerTurn = false;
                    break;
                }
                default:
                    throw new Exception($"Apply for ItemType {Item.ItemType} has not been implemented");
            }
            
            
            
            return Result(CommandResult.Success(this, message, subsequentCommand));
        }
    }
}
