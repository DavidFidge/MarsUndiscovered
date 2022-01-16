using FrigidRogue.MonoGame.Core.UserInterface;

using MarsUndiscovered.UserInterface.Views;

using MediatR;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Open Game Inventory", DefaultKey = Keys.I)]
    [ActionMap(Name = "Equip", DefaultKey = Keys.E)]
    [ActionMap(Name = "Unequip", DefaultKey = Keys.R)]
    [ActionMap(Name = "Drop", DefaultKey = Keys.D)]
    public class OpenGameInventoryRequest : IRequest
    {
        public InventoryMode InventoryMode { get; }

        public OpenGameInventoryRequest(InventoryMode inventoryMode)
        {
            InventoryMode = inventoryMode;
        }
    }
}