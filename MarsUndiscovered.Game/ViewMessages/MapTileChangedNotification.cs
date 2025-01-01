using FrigidRogue.MonoGame.Core.Components.Mediator;
using SadRogue.Primitives;

namespace MarsUndiscovered.Game.ViewMessages
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