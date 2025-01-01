using FrigidRogue.MonoGame.Core.UserInterface;
using Microsoft.Xna.Framework.Input;
using SadRogue.Primitives;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Move Square Choice Up Request", DefaultKey = Keys.NumPad8)]
    public class MoveSquareChoiceSelectionUpRequest : MoveSquareChoiceRequest
    {
        public MoveSquareChoiceSelectionUpRequest() : base(Direction.Up)
        {
        }
    }
}