using FrigidRogue.MonoGame.Core.Components.Mediator;
using SadRogue.Primitives;

namespace MarsUndiscovered.Messages
{
    public class MoveRequest : IRequest
    {
        public Direction Direction { get; }

        public MoveRequest(Direction direction)
        {
            Direction = direction;
        }
    }
}