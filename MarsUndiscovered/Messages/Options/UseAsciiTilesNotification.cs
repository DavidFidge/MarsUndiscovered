using MediatR;

namespace MarsUndiscovered.Messages
{
    public class UseAsciiTilesNotification : INotification
    {
        public bool UseAsciiTiles { get; }

        public UseAsciiTilesNotification(bool useAsciiTiles)
        {
            UseAsciiTiles = useAsciiTiles;
        }
    }
}