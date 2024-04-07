using FrigidRogue.MonoGame.Core.UserInterface;
using MediatR;
using Microsoft.Xna.Framework.Input;
using SadRogue.Primitives;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Square Choice Next Target Request", DefaultKey = Keys.Tab)]
    [ActionMap(Name = "Square Choice Next Target Request 2", DefaultKey = Keys.F)]
    public class SquareChoiceNextTargetRequest : IRequest
    {
    }
}