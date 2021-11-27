using MediatR;

namespace MarsUndiscovered.Messages
{
    public class Rotate3DViewRequest : IRequest
    {
        public float XRotation { get; }
        public float ZRotation { get; }

        public Rotate3DViewRequest(float xRotation, float zRotation)
        {
            XRotation = xRotation;
            ZRotation = zRotation;
        }
    }
}