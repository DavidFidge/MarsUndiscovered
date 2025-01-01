using FrigidRogue.MonoGame.Core.Components.Mediator;
using FrigidRogue.MonoGame.Core.UserInterface;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Open Console", DefaultKey = Keys.OemTilde)]
    public class OpenConsoleRequest : IRequest
    {
    }
}