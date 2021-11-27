using DavidFidge.MonoGame.Core.UserInterface;

namespace MarsUndiscovered.Messages
{
    public class VideoOptionsVerticalSyncToggle : ToggleRequest
    {
        public VideoOptionsVerticalSyncToggle(bool isChecked) : base(isChecked)
        {
        }
    }
}