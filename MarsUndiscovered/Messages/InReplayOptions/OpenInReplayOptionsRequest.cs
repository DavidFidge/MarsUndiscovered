using FrigidRogue.MonoGame.Core.UserInterface;

using MediatR;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Open In-Replay Options", DefaultKey = Keys.Escape)]
    public class OpenInReplayOptionsRequest : IRequest
    {
    }
}