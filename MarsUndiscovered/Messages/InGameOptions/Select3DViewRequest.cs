using MediatR;

namespace MarsUndiscovered.Messages
{
    public class Select3DViewRequest : IRequest
    {
        public Select3DViewRequest(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }
    }
}