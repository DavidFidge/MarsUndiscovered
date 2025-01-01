
using FrigidRogue.MonoGame.Core.Components.Mediator;

namespace MarsUndiscovered.Messages
{
    public abstract class BaseClickViewRequest : IRequest
    {
        public BaseClickViewRequest(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }
    }
}