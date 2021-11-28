using FrigidRogue.MonoGame.Core.Graphics.Camera;

using MediatR;

namespace MarsUndiscovered.Messages
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