using FrigidRogue.MonoGame.Core.Components.Mediator;
using FrigidRogue.MonoGame.Core.UserInterface;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages.Console
{
    [ActionMap(Name = "Next Console Command", DefaultKey = Keys.Down)]
    public class RecallConsoleHistoryForwardRequest : IRequest
    {
    }
}