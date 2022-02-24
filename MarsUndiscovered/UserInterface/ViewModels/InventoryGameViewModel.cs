using System.Linq;

using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public class InventoryGameViewModel : BaseInventoryViewModel<InventoryGameData>
    {
        public void EquipRequest(Keys requestKey)
        {
            var item = _inventoryItems.FirstOrDefault(i => i.Key == requestKey);

            if (item != null)
            {
                GameWorldEndpoint.GameWorld.EquipItemRequest(requestKey);
                Mediator.Send(new CloseGameInventoryRequest());
                Mediator.Publish(new RefreshViewNotification());
            }
        }

        public void UnequipRequest(Keys requestKey)
        {
            var item = _inventoryItems.FirstOrDefault(i => i.Key == requestKey);

            if (item != null)
            {
                GameWorldEndpoint.GameWorld.UnequipItemRequest(requestKey);
                Mediator.Send(new CloseGameInventoryRequest());
                Mediator.Publish(new RefreshViewNotification());
            }
        }

        public void DropRequest(Keys requestKey)
        {
            var item = _inventoryItems.FirstOrDefault(i => i.Key == requestKey);

            if (item != null)
            {
                GameWorldEndpoint.GameWorld.DropItemRequest(requestKey);
                Mediator.Send(new CloseGameInventoryRequest());
                Mediator.Publish(new RefreshViewNotification());
            }
        }
    }
}