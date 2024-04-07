using FrigidRogue.MonoGame.Core.UserInterface;
using MediatR;
using Microsoft.Xna.Framework.Input;
using SadRogue.Primitives;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Move Square Choice Down Right Request", DefaultKey = Keys.NumPad3)]
    public class MoveSquareChoiceSelectionDownRightRequest : MoveSquareChoiceRequest
    {
        public MoveSquareChoiceSelectionDownRightRequest() : base(Direction.DownRight)
        {
        }
    }
}