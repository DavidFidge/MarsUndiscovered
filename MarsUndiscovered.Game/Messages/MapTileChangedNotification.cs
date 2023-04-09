using MediatR;

using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Messages
{
    public class MapTileChangedNotification : INotification
    {
        public Point Point { get; }

        public MapTileChangedNotification(Point point)
        {
            Point = point;
        }
    }
}