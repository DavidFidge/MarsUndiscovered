using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;

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
            DoRequest(requestKey, requestKey1 => GameWorldEndpoint.ApplyItemRequest(requestKey1));
        }

        private void DoRequest(Keys requestKey, Action<Keys> action)
        {
            var item = _inventoryItems.FirstOrDefault(i => i.Key == requestKey);

            if (item != null)
            {
                action(requestKey);
                Mediator.Send(new CloseGameInventoryRequest());
                Mediator.Publish(new RefreshViewNotification());
            }
        }

        public void EnchantItemRequest(Keys requestKey)
        {
            DoRequest(requestKey, GameWorldEndpoint.EnchantItemRequest);
        }
    }
}
