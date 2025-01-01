
using FrigidRogue.MonoGame.Core.Components.Mediator;

namespace MarsUndiscovered.Messages
{
    public class SquareChoiceMouseHoverViewRequest : IRequest
    {
        public SquareChoiceMouseHoverViewRequest(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }
    }
}