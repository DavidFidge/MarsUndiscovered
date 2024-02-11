using FrigidRogue.MonoGame.Core.UserInterface;

using MediatR;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = UiConstants.InventoryItemSelectionCycleRequestPrevious, DefaultKey = Keys.Up)]
    [ActionMap(Name = UiConstants.InventoryItemSelectionCycleRequestNext, DefaultKey = Keys.Down)]
    public class InventoryItemSelectionCycleRequest : IRequest
    {
        public InventoryItemSelectionCycleRequestType InventoryItemSelectionCycleRequestType { get; }

        public InventoryItemSelectionCycleRequest(InventoryItemSelectionCycleRequestType inventoryItemSelectionCycleRequestType)
        {
            InventoryItemSelectionCycleRequestType = inventoryItemSelectionCycleRequestType;
        }
    }
}