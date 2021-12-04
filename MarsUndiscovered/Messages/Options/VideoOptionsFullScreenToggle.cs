using FrigidRogue.MonoGame.Core.UserInterface;

namespace MarsUndiscovered.Messages
{
    public class VideoOptionsFullScreenToggle : ToggleRequest
    {
        public VideoOptionsFullScreenToggle(bool isChecked) : base(isChecked)
        {
        }
    }
}