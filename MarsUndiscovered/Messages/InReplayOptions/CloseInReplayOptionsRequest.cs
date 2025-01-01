using FrigidRogue.MonoGame.Core.Components.Mediator;
using FrigidRogue.MonoGame.Core.UserInterface;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Close In-Replay Options", DefaultKey = Keys.Escape)]
    public class CloseInReplayOptionsRequest : IRequest
    {
    }
}