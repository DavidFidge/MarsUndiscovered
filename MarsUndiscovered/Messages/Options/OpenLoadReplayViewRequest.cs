using FrigidRogue.MonoGame.Core.UserInterface;
using MediatR;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Open Load Replay View", DefaultKey = Keys.R)]

    public class OpenLoadReplayViewRequest : IRequest
    {
    }
}