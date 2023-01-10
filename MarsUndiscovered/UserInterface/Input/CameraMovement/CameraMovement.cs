using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Graphics.Camera;
using FrigidRogue.MonoGame.Core.Messages;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Input.CameraMovementSpace
{
    public class CameraMovement : BaseComponent, ICameraMovement
    {
        public void MoveCamera(Keys[] keysDown)
        {
            var cameraMovementFlags = CameraMovementType.None;

            if (keysDown.Contains(Keys.Delete))
                cameraMovementFlags |= CameraMovementType.PanLeft;

            if (keysDown.Contains(Keys.PageDown))
                cameraMovementFlags |= CameraMovementType.PanRight;

            if (keysDown.Contains(Keys.Home))
                cameraMovementFlags |= CameraMovementType.PanUp;

            if (keysDown.Contains(Keys.End))
                cameraMovementFlags |= CameraMovementType.PanDown;

            if (keysDown.Contains(Keys.Q))
                cameraMovementFlags |= CameraMovementType.RotateLeft;

            if (keysDown.Contains(Keys.E))
                cameraMovementFlags |= CameraMovementType.RotateRight;

            if (keysDown.Contains(Keys.PageUp) || keysDown.Contains(Keys.R))
                cameraMovementFlags |= CameraMovementType.RotateUp;

            if (keysDown.Contains(Keys.PageDown) || keysDown.Contains(Keys.F))
                cameraMovementFlags |= CameraMovementType.RotateDown;

            if (cameraMovementFlags != CameraMovementType.None)
                Mediator.Send(new MoveViewRequest(cameraMovementFlags));

            if (keysDown.Contains(Keys.OemOpenBrackets) || keysDown.Contains(Keys.OemCloseBrackets))
            {
                var magnitude = 0;

                if (keysDown.Contains(Keys.OemOpenBrackets))
                    magnitude += 1;
                else if (keysDown.Contains(Keys.OemCloseBrackets))
                    magnitude -= 1;

                ZoomCamera(magnitude);
            }
        }

        public void ZoomCamera(int magnitude)
        {
            Mediator.Send(new ZoomViewRequest(magnitude));
        }
    }
}
