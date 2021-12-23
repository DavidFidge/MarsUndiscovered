using System;

using Point = SadRogue.Primitives.Point;

namespace MarsUndiscovered.Commands
{
    public class MoveCommandData
    {
        public Tuple<Point, Point> FromTo { get; set; }
        public uint GameObjectId { get; set; }
    }
}
