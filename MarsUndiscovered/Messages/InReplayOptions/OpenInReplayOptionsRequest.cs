using FrigidRogue.MonoGame.Core.Components.Mediator;
using FrigidRogue.MonoGame.Core.UserInterface;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Open In-Replay Options", DefaultKey = Keys.Escape)]
    public class OpenInReplayOptionsRequest : IRequest
    {
    }
}