using FrigidRogue.MonoGame.Core.Components;

using MarsUndiscovered.Interfaces;

using Microsoft.Xna.Framework;

using SadRogue.Primitives.GridViews;

using IDrawable = FrigidRogue.MonoGame.Core.Graphics.IDrawable;

using Point = SadRogue.Primitives.Point;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public class MapTileEntity : Entity, IDrawable
    {
        public IAssets Assets { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Index { get; set; }
        public int MaxHeight { get; set; }
        public float CellSize => 2f / MaxHeight;
        public bool IsFloor { get; set; }
        public bool IsWall { get; set; }

        public void Draw(Matrix view, Matrix projection, Matrix world)
        {
            if (IsWall)
            {
                Assets.WallBackgroundQuad?.Draw(view, projection, world);
                Assets.WallForegroundQuad?.Draw(view, projection, world);
            }
            else
            {
                Assets.FloorQuad?.Draw(view, projection, world);
            }
        }

        public void Initialize(int x, int y, ArrayView<bool> dataWallsFloors)
        {
            X = x;
            Y = y;
            Index = Point.ToIndex(x, y, dataWallsFloors.Width);
            MaxHeight = dataWallsFloors.Height;
            Transform.ChangeTranslation(new Vector3(X * Graphics.Assets.TileQuadWidth, Y * Graphics.Assets.TileQuadHeight, 0));
            //Transform.ChangeScale(new Vector3(CellSize, CellSize, CellSize));
            IsFloor = dataWallsFloors[x, y];
            IsWall = !dataWallsFloors[x, y];
        }
    }
}