using FrigidRogue.MonoGame.Core.UserInterface;

using MediatR;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Open Console", DefaultKey = Keys.OemTilde)]
    public class OpenConsoleRequest : IRequest
    {
    }
}