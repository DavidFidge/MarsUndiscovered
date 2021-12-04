using FrigidRogue.MonoGame.Core.Services;

using MediatR;

namespace MarsUndiscovered.Messages
{
    public class SetDisplayModeRequest : IRequest
    {
        public DisplayDimensions DisplayDimensions { get; }

        public SetDisplayModeRequest(DisplayDimensions displayDimensions)
        {
            DisplayDimensions = displayDimensions;
        }
    }
}