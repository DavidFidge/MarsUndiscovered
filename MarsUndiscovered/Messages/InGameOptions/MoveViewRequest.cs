using FrigidRogue.MonoGame.Core.Graphics.Camera;

using MediatR;

namespace MarsUndiscovered.Messages
{
    public class MoveViewRequest : IRequest
    {
        public CameraMovement CameraMovementFlags { get; }

        public MoveViewRequest(CameraMovement cameraMovementFlags)
        {
            CameraMovementFlags = cameraMovementFlags;
        }
    }
}