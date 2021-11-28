using FrigidRogue.MonoGame.Core.UserInterface;

using MediatR;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Execute Console Command", DefaultKey = Keys.Enter)]
    public class ExecuteConsoleCommandRequest : IRequest
    {
    }
}