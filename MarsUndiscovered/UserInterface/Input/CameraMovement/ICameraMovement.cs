using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Input.CameraMovementSpace
{
    public interface ICameraMovement
    {
        void MoveCamera(Keys[] keysDown);
    }
}