using Microsoft.Xna.Framework.Input;

namespace Augmented.UserInterface.Input.CameraMovementSpace
{
    public interface ICameraMovement
    {
        void MoveCamera(Keys[] keysDown);
    }
}