using System.Collections.Generic;

using GoRogue.GameFramework;

using SadRogue.Primitives;

namespace MarsUndiscovered.Components
{
    public class SeenTile
    {
        public bool HasBeenSeen { get; set; }
        public Point Point { get; set; }
        public IList<IGameObject> LastSeenGameObjects { get; set; } = new List<IGameObject>();

        public SeenTile(Point point)
        {
            Point = point;
        }
    }
}