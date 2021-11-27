using DavidFidge.MonoGame.Core.UserInterface;

namespace Augmented.Messages
{
    public class VideoOptionsVerticalSyncToggle : ToggleRequest
    {
        public VideoOptionsVerticalSyncToggle(bool isChecked) : base(isChecked)
        {
        }
    }
}