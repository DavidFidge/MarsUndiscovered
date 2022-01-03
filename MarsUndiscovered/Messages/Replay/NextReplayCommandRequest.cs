using FrigidRogue.MonoGame.Core.UserInterface;

using MediatR;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Next Replay Command", DefaultKey = Keys.Space)]
    public class NextReplayCommandRequest : IRequest
    {
    }
}