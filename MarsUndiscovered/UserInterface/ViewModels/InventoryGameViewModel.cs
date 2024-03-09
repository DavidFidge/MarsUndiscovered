using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.Views;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public class InventoryGameViewModel : BaseInventoryViewModel<InventoryGameData>
    {
        public void EquipRequest(Keys requestKey)
        {
            DoRequest(requestKey, requestKey1 => GameWorldEndpoint.EquipItemRequest(requestKey1));
        }

        public void UnequipRequest(Keys requestKey)
        {
            DoRequest(requestKey, requestKey1 => GameWorldEndpoint.UnequipItemRequest(requestKey1));
        }

        public void DropRequest(Keys requestKey)
        {
            DoRequest(requestKey, requestKey1 => GameWorldEndpoint.DropItemRequest(requestKey1));
        }

        public void ApplyRequest(Keys requestKey)
        {
            var commandResults = DoRequest(requestKey, requestKey1 => GameWorldEndpoint.ApplyItemRequest(requestKey1));
            
            foreach (var commandResult in commandResults)
            {
                if (commandResult.Result == CommandResultEnum.Success && commandResult.Command.RequiresPlayerInput)
                {
                    var applyItemCommand = (ApplyItemCommand)commandResult.Command;

                    if (applyItemCommand.Item.ItemType == ItemType.EnhancementBots)
                    {
                        Mediator.Send(new OpenGameInventoryRequest(InventoryMode.Enchant));
                    }
                }
            }
        }

        private IList<CommandResult> DoRequest(Keys requestKey, Func<Keys, IList<CommandResult>> action)
        {
            var item = _inventoryItems.FirstOrDefault(i => i.Key == requestKey);

            if (item != null)
            {
                var commandResults = action(requestKey);
                
                if (commandResults.Any(c => c.Result == CommandResultEnum.Success))
                    Mediator.Send(new CloseGameInventoryRequest());
                
                Mediator.Publish(new RefreshViewNotification());

                return commandResults;
            }

            return new List<CommandResult>();
        }

        public void EnchantItemRequest(Keys requestKey)
        {
            DoRequest(requestKey, requestKey => GameWorldEndpoint.EnchantItemRequest(requestKey));
        }
        
        public void IdentifyItemRequest(Keys requestKey)
        {
            DoRequest(requestKey, requestKey => GameWorldEndpoint.IdentifyItemRequest(requestKey));
        }

        public void ClosingInventoryNoAction(InventoryMode inventoryMode)
        {
            if (inventoryMode == InventoryMode.Identify)
                GameWorldEndpoint.CancelIdentify();
        }

        public void AssignHotBarItem(Keys inventoryItemKey, Keys requestKey)
        {
            GameWorldEndpoint.AssignHotBarItem(inventoryItemKey, requestKey);
        }
    }
}
