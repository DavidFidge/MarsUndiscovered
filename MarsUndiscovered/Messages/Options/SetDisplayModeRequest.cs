using FrigidRogue.MonoGame.Core.Components.Mediator;
using FrigidRogue.MonoGame.Core.Services;

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