using DavidFidge.MonoGame.Core.Graphics.Camera;

using MediatR;

namespace Augmented.Messages
{
    public class Move3DViewRequest : IRequest
    {
        public CameraMovement CameraMovementFlags { get; }

        public Move3DViewRequest(CameraMovement cameraMovementFlags)
        {
            CameraMovementFlags = cameraMovementFlags;
        }
    }
}