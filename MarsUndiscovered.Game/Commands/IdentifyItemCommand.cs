using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Commands
{
    public class IdentifyItemCommand : BaseMarsGameActionCommand<IdentifyItemCommandSaveData>
    {
        public Item Item => GameWorld.Items[_data.ItemId];

        public IdentifyItemCommand(IGameWorld gameWorld) : base(gameWorld)
        {
            EndsPlayerTurn = true;
        }

        public void Initialise(Item item)
        {
            _data.ItemId = item.ID;
        }

        protected override CommandResult ExecuteInternal()
        {
            var isIdentified = GameWorld.Inventory.IsIdentified(Item);

            if (isIdentified)
            {
                EndsPlayerTurn = false;
                
                return Result(CommandResult.NoMove(this,
                    $"The {GameWorld.Inventory.ItemTypeDiscoveries.GetInventoryDescriptionWithoutPrefix(Item)} is already identified."));
            }

            var undiscoveredDescription =
                GameWorld.Inventory.ItemTypeDiscoveries.GetInventoryDescriptionWithoutPrefixAndWithoutStatus(Item);
            
            GameWorld.Inventory.ItemTypeDiscoveries.SetItemTypeDiscovered(Item);
            Item.ItemDiscovery.IsItemSpecialDiscovered = true;
            Item.ItemDiscovery.IsEnchantLevelDiscovered = true;

            var discoveredDescription = GameWorld.Inventory.ItemTypeDiscoveries.GetInventoryDescriptionWithoutPrefixAndWithoutStatus(Item);
            
            var message =
                $"The {undiscoveredDescription} is a {discoveredDescription}!";
            
            var commandResult = CommandResult.Success(this, message);

            return Result(commandResult);
        }

        protected override void UndoInternal()
        {
        }
    }
}
