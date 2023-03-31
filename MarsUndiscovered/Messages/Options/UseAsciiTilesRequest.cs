using MediatR;

namespace MarsUndiscovered.Messages
{
    public class UseAsciiTilesRequest : IRequest
    {
        public bool UseAsciiTiles { get; }

        public UseAsciiTilesRequest(bool useAsciiTiles)
        {
            UseAsciiTiles = useAsciiTiles;
        }
    }
}