using DavidFidge.MonoGame.Core.UserInterface;

using MediatR;

using Microsoft.Xna.Framework.Input;

namespace Augmented.Messages.Console
{
    [ActionMap(Name = "Previous Console Command", DefaultKey = Keys.Up)]
    public class RecallConsoleHistoryBackRequest : IRequest<Unit>
    {
    }
}