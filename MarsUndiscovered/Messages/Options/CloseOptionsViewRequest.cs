using DavidFidge.MonoGame.Core.UserInterface;

using MediatR;

using Microsoft.Xna.Framework.Input;

namespace Augmented.Messages
{
    [ActionMap(Name = "Close Options", DefaultKey = Keys.Escape)]
    public class CloseOptionsViewRequest : IRequest
    {
    }
}