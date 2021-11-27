using DavidFidge.MonoGame.Core.UserInterface;

using MediatR;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages.Console
{
    [ActionMap(Name = "Next Console Command", DefaultKey = Keys.Down)]
    public class RecallConsoleHistoryForwardRequest : IRequest<Unit>
    {
    }
}