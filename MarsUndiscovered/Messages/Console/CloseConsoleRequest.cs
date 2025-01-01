using FrigidRogue.MonoGame.Core.Components.Mediator;
using FrigidRogue.MonoGame.Core.UserInterface;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(DefaultKey = Keys.OemTilde, Name = "Close Console")]
    [ActionMap(DefaultKey = Keys.Escape, Name = "Close Console 2")]
    public class CloseConsoleRequest : IRequest
    {
    }
}