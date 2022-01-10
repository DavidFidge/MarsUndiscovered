using MediatR;

namespace MarsUndiscovered.Messages
{
    public class MouseHoverViewRequest : IRequest
    {
        public MouseHoverViewRequest(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }
    }
}