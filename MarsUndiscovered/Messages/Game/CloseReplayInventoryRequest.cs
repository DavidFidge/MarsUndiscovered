using FrigidRogue.MonoGame.Core.UserInterface;
using MediatR;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Close Replay Inventory", DefaultKey = Keys.Escape)]
    public class CloseReplayInventoryRequest : IRequest
    {
    }
}