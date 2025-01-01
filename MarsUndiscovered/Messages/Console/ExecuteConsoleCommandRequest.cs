using FrigidRogue.MonoGame.Core.Components.Mediator;
using FrigidRogue.MonoGame.Core.UserInterface;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Execute Console Command", DefaultKey = Keys.Enter)]
    public class ExecuteConsoleCommandRequest : IRequest
    {
    }
}