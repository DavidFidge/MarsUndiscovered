using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Graphics;
using FrigidRogue.MonoGame.Core.Graphics.Quads;

using MarsUndiscovered.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Point = SadRogue.Primitives.Point;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public class GoalMapEntity : Entity, ISpriteBatchDrawable
    {
        public IAssets Assets { get; set; }
        public Point Position { get; set; }
        public GoalMapTileTexture GoalMapTileTexture { get; set; }

        public string Text { get; set; }

        public bool IsVisible { get; set; }
        public void SpriteBatchDraw(SpriteBatch spriteBatch)
        {
            if (IsVisible)
            {
                GoalMapTileTexture.Text = Text;
                
                var drawRegion = new Rectangle(Position.X * Constants.TileWidth, Position.Y * Constants.TileHeight,
                    Constants.TileWidth, Constants.TileHeight);

                GoalMapTileTexture.SpriteBatchDraw(spriteBatch, drawRegion);
            }
        }

        public void Initialize(Point position)
        {
            Position = position;
            Transform.ChangeTranslation(new Vector3(Position.X * Constants.TileQuadWidth, -Position.Y * Constants.TileQuadHeight, 0));
            GoalMapTileTexture = Assets.GoalMapTileTexture;
        }
    }
}