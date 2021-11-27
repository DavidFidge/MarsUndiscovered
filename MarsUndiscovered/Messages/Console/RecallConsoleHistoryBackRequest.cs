using DavidFidge.MonoGame.Core.UserInterface;

using MediatR;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages.Console
{
    [ActionMap(Name = "Previous Console Command", DefaultKey = Keys.Up)]
    public class RecallConsoleHistoryBackRequest : IRequest<Unit>
    {
    }
}