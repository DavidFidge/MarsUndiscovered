using MediatR;

using SadRogue.Primitives;

namespace MarsUndiscovered.Messages
{
    public class MoveSquareChoiceRequest : IRequest
    {
        public Direction Direction { get; }

        public MoveSquareChoiceRequest(Direction direction)
        {
            Direction = direction;
        }
    }
}