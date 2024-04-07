using FrigidRogue.MonoGame.Core.UserInterface;
using MediatR;
using Microsoft.Xna.Framework.Input;
using SadRogue.Primitives;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Square Choice Select Square Request", DefaultKey = Keys.Enter)]
    public class SquareChoiceSelectSquareRequest : IRequest
    {
    }
}