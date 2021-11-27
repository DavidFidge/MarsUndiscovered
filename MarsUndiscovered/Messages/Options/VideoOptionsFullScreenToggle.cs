using DavidFidge.MonoGame.Core.UserInterface;

namespace Augmented.Messages
{
    public class VideoOptionsFullScreenToggle : ToggleRequest
    {
        public VideoOptionsFullScreenToggle(bool isChecked) : base(isChecked)
        {
        }
    }
}