using FrigidRogue.MonoGame.Core.Components.Mediator;
using FrigidRogue.MonoGame.Core.UserInterface;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Close Replay Inventory", DefaultKey = Keys.Escape)]
    public class CloseReplayInventoryRequest : IRequest
    {
    }
}