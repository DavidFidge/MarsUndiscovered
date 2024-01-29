using FrigidRogue.MonoGame.Core.UserInterface;

using MediatR;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = Constants.InventoryItemSelectionCycleRequestPrevious, DefaultKey = Keys.Up)]
    [ActionMap(Name = Constants.InventoryItemSelectionCycleRequestNext, DefaultKey = Keys.Down)]
    public class InventoryItemSelectionCycleRequest : IRequest
    {
        public InventoryItemSelectionCycleRequestType InventoryItemSelectionCycleRequestType { get; }

        public InventoryItemSelectionCycleRequest(InventoryItemSelectionCycleRequestType inventoryItemSelectionCycleRequestType)
        {
            InventoryItemSelectionCycleRequestType = inventoryItemSelectionCycleRequestType;
        }
    }
}