using FrigidRogue.MonoGame.Core.Components.Mediator;
using FrigidRogue.MonoGame.Core.UserInterface;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Next Replay Command", DefaultKey = Keys.Space)]
    public class NextReplayCommandRequest : IRequest
    {
    }
}