using MediatR;

namespace MarsUndiscovered.Messages
{
    public class Zoom3DViewRequest : IRequest
    {
        public int Difference { get; }

        public Zoom3DViewRequest(int difference)
        {
            Difference = difference;
        }
    }
}