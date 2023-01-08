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
            MapTileTexture = Assets.FieldOfViewUnrevealedTexture;
            IsVisible = true;
            HasBeenSeen = false;
        }

        public void SetFieldOfViewHasBeenSeen()
        {
            MapTileTexture = Assets.FieldOfViewHasBeenSeenTexture;
            IsVisible = true;
            HasBeenSeen = true;
        }
    }
}