using FrigidRogue.MonoGame.Core.UserInterface;
using Microsoft.Xna.Framework.Input;
using SadRogue.Primitives;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Move Square Choice Down Left Request", DefaultKey = Keys.NumPad1)]
    public class MoveSquareChoiceSelectionDownLeftRequest : MoveSquareChoiceRequest
    {
        public MoveSquareChoiceSelectionDownLeftRequest() : base(Direction.DownLeft)
        {
        }
    }
}