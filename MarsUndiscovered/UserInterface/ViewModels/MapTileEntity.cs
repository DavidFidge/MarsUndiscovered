using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Graphics.Quads;

using GoRogue.GameFramework;

using MarsUndiscovered.Components;
using MarsUndiscovered.Interfaces;

using Microsoft.Xna.Framework;

using IDrawable = FrigidRogue.MonoGame.Core.Graphics.IDrawable;

using Point = SadRogue.Primitives.Point;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public class MapTileEntity : Entity, IDrawable
    {
        public IAssets Assets { get; set; }
        public Point Point { get; set; }
        public int MaxHeight { get; set; }
        public BaseQuadTemplate BackgroundQuad { get; private set; }
        public BaseQuadTemplate ForegroundQuad { get; private set; }
        public bool IsVisible { get; set; } = true;

        public void Draw(Matrix view, Matrix projection, Matrix world)
        {
            if (BackgroundQuad != null)
                BackgroundQuad.Draw(view, projection, world);

            if (ForegroundQuad != null)
                ForegroundQuad.Draw(view, projection, world);
        }

        public void Initialize(IGameObject gameObject)
        {
            Point = gameObject.Position;
            MaxHeight = gameObject.CurrentMap.Height;
            Transform.ChangeTranslation(new Vector3(Point.X * Graphics.Assets.TileQuadWidth, -Point.Y * Graphics.Assets.TileQuadHeight, 0));

            switch (gameObject)
            {
                case Floor _:
                    ForegroundQuad = Assets.FloorQuad;
                    break;

                case Wall _:
                    ForegroundQuad = Assets.WallForegroundQuad;
                    BackgroundQuad = Assets.WallBackgroundQuad;
                    break;

                case Player _:
                    ForegroundQuad = Assets.PlayerForegroundQuad;
                    break;
            }
        }
    }
}