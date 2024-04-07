using FrigidRogue.MonoGame.Core.UserInterface;
using MediatR;
using Microsoft.Xna.Framework.Input;
using SadRogue.Primitives;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Move Square Choice Right Request", DefaultKey = Keys.NumPad6)]
    public class MoveSquareChoiceSelectionRightRequest : MoveSquareChoiceRequest
    {
        public MoveSquareChoiceSelectionRightRequest() : base(Direction.Right)
        {
        }
    }
}