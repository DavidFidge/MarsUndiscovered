using MediatR;

namespace MarsUndiscovered.Messages
{
    public class RightClickViewRequest : IRequest
    {
        public RightClickViewRequest(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }
    }
}