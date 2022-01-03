using System;

using MediatR;

using SadRogue.Primitives;

namespace MarsUndiscovered.Messages
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