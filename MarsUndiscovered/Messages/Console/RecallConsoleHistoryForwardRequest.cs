using DavidFidge.MonoGame.Core.UserInterface;

using MediatR;

using Microsoft.Xna.Framework.Input;

namespace Augmented.Messages.Console
{
    [ActionMap(Name = "Next Console Command", DefaultKey = Keys.Down)]
    public class RecallConsoleHistoryForwardRequest : IRequest<Unit>
    {
    }
}