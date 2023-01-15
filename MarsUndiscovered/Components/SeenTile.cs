using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using GoRogue.GameFramework;

using MarsUndiscovered.Components.SaveData;

using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

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

        public void Reset()
        {
            HasBeenSeen = false;
            LastSeenGameObjects.Clear();
        }

        public IMemento<SeenTileSaveData> GetSaveState()
        {
            var memento = new Memento<SeenTileSaveData>(new SeenTileSaveData());

            memento.State.Point = Point;
            memento.State.HasBeenSeen = HasBeenSeen;
            memento.State.LastSeenGameObjectIds = LastSeenGameObjects.Select(s => s.ID).ToList();

            return memento;
        }

        public void SetLoadState(IMemento<SeenTileSaveData> memento)
        {
            Point = memento.State.Point;
            HasBeenSeen = memento.State.HasBeenSeen;
            // LastSeenGameObjectIds has to be populated later if using this method
        }

        public void SetLoadState(IMemento<SeenTileSaveData> memento, IDictionary<uint, IGameObject> gameObjects)
        {
            SetLoadState(memento);

            LastSeenGameObjects = memento.State.LastSeenGameObjectIds
                .Select(o => gameObjects[o])
                .ToList();
        }

        public static ArrayView<SeenTile> CreateArrayViewFromMap(MarsMap map)
        {
            var seenTiles = map.Positions()
                .ToEnumerable()
                .Select(p => new SeenTile(p))
                .ToArray();

            var seenTilesArrayView = new ArrayView<SeenTile>(seenTiles, map.Width);

            return seenTilesArrayView;
        }

        public static void ResetSeenTiles(ArrayView<SeenTile> seenTiles)
        {
            foreach(var tile in seenTiles.ToArray())
                tile.Reset();
        }
    }
}
