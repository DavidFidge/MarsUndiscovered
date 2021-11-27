using DavidFidge.MonoGame.Core.UserInterface;

using MediatR;

using Microsoft.Xna.Framework.Input;

namespace Augmented.Messages
{
    [ActionMap(Name = "Execute Console Command", DefaultKey = Keys.Enter)]
    public class ExecuteConsoleCommandRequest : IRequest
    {
    }
}