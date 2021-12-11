using MediatR;

namespace MarsUndiscovered.Messages
{
    public class LeftClickViewRequest : IRequest
    {
        public LeftClickViewRequest(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }
    }
}