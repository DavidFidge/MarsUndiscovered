using FrigidRogue.MonoGame.Core.UserInterface;
using MediatR;
using Microsoft.Xna.Framework.Input;
using SadRogue.Primitives;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Move Square Choice Down Request", DefaultKey = Keys.NumPad2)]
    public class MoveSquareChoiceSelectionDownRequest : MoveSquareChoiceRequest
    {
        public MoveSquareChoiceSelectionDownRequest() : base(Direction.Down)
        {
        }
    }
}