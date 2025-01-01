using FrigidRogue.MonoGame.Core.Components.Mediator;
using FrigidRogue.MonoGame.Core.UserInterface;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages.Console
{
    [ActionMap(Name = "Previous Console Command", DefaultKey = Keys.Up)]
    public class RecallConsoleHistoryBackRequest : IRequest
    {
    }
}