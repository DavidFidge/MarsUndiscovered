using FrigidRogue.MonoGame.Core.UserInterface;

using MediatR;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Inventory Item A", DefaultKey = Keys.A)]
    [ActionMap(Name = "Inventory Item B", DefaultKey = Keys.B)]
    [ActionMap(Name = "Inventory Item C", DefaultKey = Keys.C)]
    [ActionMap(Name = "Inventory Item D", DefaultKey = Keys.D)]
    [ActionMap(Name = "Inventory Item E", DefaultKey = Keys.E)]
    [ActionMap(Name = "Inventory Item F", DefaultKey = Keys.F)]
    [ActionMap(Name = "Inventory Item G", DefaultKey = Keys.G)]
    [ActionMap(Name = "Inventory Item H", DefaultKey = Keys.H)]
    [ActionMap(Name = "Inventory Item I", DefaultKey = Keys.I)]
    [ActionMap(Name = "Inventory Item J", DefaultKey = Keys.J)]
    [ActionMap(Name = "Inventory Item K", DefaultKey = Keys.K)]
    [ActionMap(Name = "Inventory Item L", DefaultKey = Keys.L)]
    [ActionMap(Name = "Inventory Item M", DefaultKey = Keys.M)]
    [ActionMap(Name = "Inventory Item N", DefaultKey = Keys.N)]
    [ActionMap(Name = "Inventory Item O", DefaultKey = Keys.O)]
    [ActionMap(Name = "Inventory Item P", DefaultKey = Keys.P)]
    [ActionMap(Name = "Inventory Item Q", DefaultKey = Keys.Q)]
    [ActionMap(Name = "Inventory Item R", DefaultKey = Keys.R)]
    [ActionMap(Name = "Inventory Item S", DefaultKey = Keys.S)]
    [ActionMap(Name = "Inventory Item T", DefaultKey = Keys.T)]
    [ActionMap(Name = "Inventory Item U", DefaultKey = Keys.U)]
    [ActionMap(Name = "Inventory Item V", DefaultKey = Keys.V)]
    [ActionMap(Name = "Inventory Item W", DefaultKey = Keys.W)]
    [ActionMap(Name = "Inventory Item X", DefaultKey = Keys.X)]
    [ActionMap(Name = "Inventory Item Y", DefaultKey = Keys.Y)]
    [ActionMap(Name = "Inventory Item Z", DefaultKey = Keys.Z)]
    public class InventoryItemSelectionRequest : IRequest
    {
        public Keys Key { get; }

        public InventoryItemSelectionRequest(Keys key)
        {
            Key = key;
        }
    }
}