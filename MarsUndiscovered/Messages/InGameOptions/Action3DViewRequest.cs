using MediatR;

namespace Augmented.Messages
{
    public class Action3DViewRequest : IRequest
    {
        public Action3DViewRequest(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }
    }
}