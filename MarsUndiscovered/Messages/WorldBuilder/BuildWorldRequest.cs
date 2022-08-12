using FrigidRogue.MonoGame.Core.UserInterface;

using MediatR;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Build New World", DefaultKey = Keys.B)]
    public class BuildWorldRequest : IRequest
    {
    }
}