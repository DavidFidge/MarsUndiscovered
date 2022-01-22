using System.Collections.Generic;
using System.Linq;

using GoRogue.GameFramework;

using SadRogue.Primitives;

namespace MarsUndiscovered.Components
{
    public class SeenTile
    {
        public bool HasBeenSeen { get; set; }
        public Point Point { get; set; }
        public IList<IGameObject> LastSeenGameObjects { get; set; } = new List<IGameObject>();
        public bool HasUndroppedItem => LastSeenGameObjects.Any(o => o is Item { HasBeenDropped: false });

        public SeenTile(Point point)
        {
            Point = point;
        }
    }
}