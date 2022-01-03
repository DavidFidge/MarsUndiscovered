using FrigidRogue.MonoGame.Core.UserInterface;
using MediatR;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Close Load Replay View", DefaultKey = Keys.Escape)]

    public class CloseLoadReplayViewRequest : IRequest
    {
    }
}