using DavidFidge.MonoGame.Core.UserInterface;

using MediatR;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(DefaultKey = Keys.OemTilde, Name = "Close Console")]
    public class CloseConsoleRequest : IRequest
    {
    }
}