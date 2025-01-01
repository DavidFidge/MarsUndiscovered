using FrigidRogue.MonoGame.Core.Components.Mediator;
using FrigidRogue.MonoGame.Core.Services;

namespace MarsUndiscovered.Messages
{
    public class SetRenderResolutionRequest : IRequest
    {
        public RenderResolution RenderResolution { get; }

        public SetRenderResolutionRequest(RenderResolution renderResolution)
        {
            RenderResolution = renderResolution;
        }
    }
}