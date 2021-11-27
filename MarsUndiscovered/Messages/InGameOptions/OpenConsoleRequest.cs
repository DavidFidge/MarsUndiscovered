using DavidFidge.MonoGame.Core.UserInterface;

using MediatR;

using Microsoft.Xna.Framework.Input;

namespace Augmented.Messages
{
    [ActionMap(Name = "Open Console", DefaultKey = Keys.OemTilde)]
    public class OpenConsoleRequest : IRequest
    {
    }
}