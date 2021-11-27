using DavidFidge.MonoGame.Core.UserInterface;

using MediatR;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Close Options", DefaultKey = Keys.Escape)]
    public class CloseOptionsViewRequest : IRequest
    {
    }
}