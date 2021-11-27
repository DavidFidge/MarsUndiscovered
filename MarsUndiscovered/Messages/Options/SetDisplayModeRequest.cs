using DavidFidge.MonoGame.Core.Services;

using MediatR;

namespace Augmented.Messages
{
    public class SetDisplayModeRequest : IRequest
    {
        public DisplayMode DisplayMode { get; }

        public SetDisplayModeRequest(DisplayMode displayMode)
        {
            DisplayMode = displayMode;
        }
    }
}