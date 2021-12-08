using FrigidRogue.MonoGame.Core.Services;

using MediatR;

namespace MarsUndiscovered.Messages
{
    public class SetDisplayModeRequest : IRequest
    {
        public DisplayDimension DisplayDimension { get; }

        public SetDisplayModeRequest(DisplayDimension displayDimension)
        {
            DisplayDimension = displayDimension;
        }
    }
}