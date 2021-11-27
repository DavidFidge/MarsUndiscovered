using DavidFidge.MonoGame.Core.UserInterface;

using MediatR;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Close In-Game Options", DefaultKey = Keys.Escape)]
    public class CloseInGameOptionsRequest : IRequest
    {
    }
}