using FrigidRogue.MonoGame.Core.UserInterface;

using MediatR;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Build World Request", DefaultKey = Keys.B)]
    public class BuildWorldRequest : IRequest
    {
    }
}