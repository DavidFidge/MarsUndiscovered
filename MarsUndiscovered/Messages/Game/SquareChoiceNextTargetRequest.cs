using FrigidRogue.MonoGame.Core.Components.Mediator;
using FrigidRogue.MonoGame.Core.UserInterface;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Square Choice Next Target Request", DefaultKey = Keys.Tab)]
    [ActionMap(Name = "Square Choice Next Target Request 2", DefaultKey = Keys.F)]
    public class SquareChoiceNextTargetRequest : IRequest
    {
    }
}