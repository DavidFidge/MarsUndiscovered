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
        public MapTileQuad MapTileQuad { get; set; }
        public bool IsVisible { get; set; } = true;

        public void Draw(Matrix view, Matrix projection, Matrix world)
        {
            MapTileQuad.Draw(view, projection, world);
        }

        public void Initialize(IGameObject gameObject)
        {
            Point = gameObject.Position;
            MaxHeight = gameObject.CurrentMap.Height;
            Transform.ChangeTranslation(new Vector3(Point.X * Graphics.Assets.TileQuadWidth, -Point.Y * Graphics.Assets.TileQuadHeight, 0));

            switch (gameObject)
            {
                case Floor _:
                    MapTileQuad = Assets.Floor;
                    break;

                case Wall _:
                    MapTileQuad = Assets.Wall;
                    break;

                case Player _:
                    MapTileQuad = Assets.Player;
                    break;
            }
        }
    }
}