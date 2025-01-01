using FrigidRogue.MonoGame.Core.Components.Mediator;
using FrigidRogue.MonoGame.Core.UserInterface;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Exit Game", DefaultKey = Keys.X)]
    public class QuitToTitleRequest : IRequest
    {
    }
}