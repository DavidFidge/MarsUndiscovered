using MarsUndiscovered.Game.Components;
using MediatR;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Game.ViewMessages
{
    public class FieldOfViewChangedNotification : INotification
    {
        public IEnumerable<Point> NewlyVisibleTiles { get; }
        public IEnumerable<Point> NewlyHiddenTiles { get; }
        public ArrayView<SeenTile> SeenTiles { get; }

        public FieldOfViewChangedNotification(
            IEnumerable<Point> newlyVisiblePoints,
            IEnumerable<Point> newlyHiddenPoints,
            ArrayView<SeenTile> seenTiles
        )
        {
            NewlyVisibleTiles = newlyVisiblePoints.ToArray();
            NewlyHiddenTiles = newlyHiddenPoints.ToArray();
            SeenTiles = seenTiles;
        }
    }
}
