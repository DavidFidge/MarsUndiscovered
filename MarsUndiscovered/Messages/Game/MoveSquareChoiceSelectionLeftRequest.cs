using FrigidRogue.MonoGame.Core.UserInterface;
using MediatR;
using Microsoft.Xna.Framework.Input;
using SadRogue.Primitives;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Move Square Choice Left Request", DefaultKey = Keys.NumPad4)]
    public class MoveSquareChoiceSelectionLeftRequest : MoveSquareChoiceRequest
    {
        public MoveSquareChoiceSelectionLeftRequest() : base(Direction.Left)
        {
        }
    }
}