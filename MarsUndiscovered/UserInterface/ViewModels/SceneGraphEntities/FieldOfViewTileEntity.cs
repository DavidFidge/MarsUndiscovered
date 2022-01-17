using Point = SadRogue.Primitives.Point;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public class FieldOfViewTileEntity : MapTileEntity
    {
        public bool HasBeenSeen { get; private set; }

        public FieldOfViewTileEntity(Point position) : base(position)
        {
        }

        public void SetFieldOfViewUnrevealed()
        {
            MapTileQuad = Assets.FieldOfViewUnrevealedQuad;
            IsVisible = true;
            HasBeenSeen = false;
        }

        public void SetFieldOfViewHasBeenSeen()
        {
            MapTileQuad = Assets.FieldOfViewHasBeenSeenQuad;
            IsVisible = true;
            HasBeenSeen = true;
        }
    }
}