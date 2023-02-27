using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public class InventoryGameViewModel : BaseInventoryViewModel<InventoryGameData>
    {
        public void EquipRequest(Keys requestKey)
        {
            DoRequest(requestKey, GameWorldEndpoint.EquipItemRequest);
        }

        public void UnequipRequest(Keys requestKey)
        {
            DoRequest(requestKey, GameWorldEndpoint.UnequipItemRequest);
        }

        public void DropRequest(Keys requestKey)
        {
            DoRequest(requestKey, GameWorldEndpoint.DropItemRequest);
        }

        public void ApplyRequest(Keys requestKey)
        {
            DoRequest(requestKey, GameWorldEndpoint.ApplyItemRequest);
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
    }
}
