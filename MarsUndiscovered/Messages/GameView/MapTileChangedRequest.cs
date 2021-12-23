using System;

using MediatR;

using SadRogue.Primitives;

namespace MarsUndiscovered.Messages
{
    public class MapTileChangedRequest : IRequest
    {
        public Point Point { get; }

        public MapTileChangedRequest(Point point)
        {
            Point = point;
        }
    }
}