using FrigidRogue.MonoGame.Core.UserInterface;
using MediatR;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Open Replay Inventory", DefaultKey = Keys.I)]
    public class OpenReplayInventoryRequest : IRequest
    {
    }
}