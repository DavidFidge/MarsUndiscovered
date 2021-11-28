using FrigidRogue.MonoGame.Core.UserInterface;

using MediatR;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "In-Game Options", DefaultKey = Keys.Escape)]
    public class OpenInGameOptionsRequest : IRequest
    {
    }
}