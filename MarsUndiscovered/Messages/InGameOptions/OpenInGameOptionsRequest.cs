using DavidFidge.MonoGame.Core.UserInterface;

using MediatR;

using Microsoft.Xna.Framework.Input;

namespace Augmented.Messages
{
    [ActionMap(Name = "In-Game Options", DefaultKey = Keys.Escape)]
    public class OpenInGameOptionsRequest : IRequest
    {
    }
}