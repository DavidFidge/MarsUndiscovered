using MarsUndiscovered.Graphics;
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
            MapTileTexture = Assets.GetMapTileTexture(TileGraphicType.FieldOfViewUnrevealedTexture.ToString());
            IsVisible = true;
            HasBeenSeen = false;
        }

        public void SetFieldOfViewHasBeenSeen()
        {
            MapTileTexture = Assets.GetMapTileTexture(TileGraphicType.FieldOfViewHasBeenSeenTexture.ToString());
            IsVisible = true;
            HasBeenSeen = true;
        }
    }
}