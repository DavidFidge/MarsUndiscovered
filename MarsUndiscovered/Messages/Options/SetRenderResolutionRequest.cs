using FrigidRogue.MonoGame.Core.Services;

using MediatR;

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