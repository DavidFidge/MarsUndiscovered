using FrigidRogue.MonoGame.Core.UserInterface;
using Microsoft.Xna.Framework.Input;
using SadRogue.Primitives;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Move Square Choice Up Left Request", DefaultKey = Keys.NumPad7)]
    public class MoveSquareChoiceSelectionUpLeftRequest : MoveSquareChoiceRequest
    {
        public MoveSquareChoiceSelectionUpLeftRequest() : base(Direction.UpLeft)
        {
        }
    }
}