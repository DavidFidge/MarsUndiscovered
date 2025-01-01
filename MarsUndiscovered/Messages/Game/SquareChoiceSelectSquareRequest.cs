using FrigidRogue.MonoGame.Core.Components.Mediator;
using FrigidRogue.MonoGame.Core.UserInterface;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Square Choice Select Square Request", DefaultKey = Keys.Enter)]
    public class SquareChoiceSelectSquareRequest : IRequest
    {
    }
}