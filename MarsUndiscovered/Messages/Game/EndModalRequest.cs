using FrigidRogue.MonoGame.Core.UserInterface;
using MediatR;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "End Modal", DefaultKey = Keys.Space)]
    public class EndModalRequest : IRequest
    {
    }
}