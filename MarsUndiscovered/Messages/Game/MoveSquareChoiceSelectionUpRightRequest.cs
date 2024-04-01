using FrigidRogue.MonoGame.Core.UserInterface;
using MediatR;
using Microsoft.Xna.Framework.Input;
using SadRogue.Primitives;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Move Square Choice Up Right Request", DefaultKey = Keys.NumPad9)]
    public class MoveSquareChoiceSelectionUpRightRequest : MoveSquareChoiceRequest
    {
        public MoveSquareChoiceSelectionUpRightRequest() : base(Direction.UpRight)
        {
        }
    }
}