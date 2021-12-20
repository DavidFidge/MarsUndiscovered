using FrigidRogue.MonoGame.Core.Components;

using GoRogue.GameFramework;

using MarsUndiscovered.Components;
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

        public void Initialize(int x, int y, IGridView<IGameObject> dataWallsFloors)
        {
            X = x;
            Y = y;
            Index = Point.ToIndex(x, y, dataWallsFloors.Width);
            MaxHeight = dataWallsFloors.Height;
            Transform.ChangeTranslation(new Vector3(X * Graphics.Assets.TileQuadWidth, Y * Graphics.Assets.TileQuadHeight, 0));
            IsFloor = dataWallsFloors[x, y] is Floor;
            IsWall = dataWallsFloors[x, y] is Wall;
        }
    }
}