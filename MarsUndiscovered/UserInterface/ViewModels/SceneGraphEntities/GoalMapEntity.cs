using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Graphics.Quads;

using MarsUndiscovered.Interfaces;

using Microsoft.Xna.Framework;

using IDrawable = FrigidRogue.MonoGame.Core.Graphics.IDrawable;
using Point = SadRogue.Primitives.Point;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public class GoalMapEntity : Entity, IDrawable
    {
        public IAssets Assets { get; set; }
        public Point Position { get; set; }
        public GoalMapQuad GoalMapQuad { get; set; }

        public string Text { get; set; }

        public bool IsVisible { get; set; }

        public void Draw(Matrix view, Matrix projection, Matrix world)
        {
            if (IsVisible)
            {
                GoalMapQuad.Text = Text;
                GoalMapQuad.Draw(view, projection, world);
            }
        }

        public void Initialize(Point position)
        {
            Position = position;
            Transform.ChangeTranslation(new Vector3(Position.X * Graphics.Assets.TileQuadWidth, -Position.Y * Graphics.Assets.TileQuadHeight, 0));
            GoalMapQuad = Assets.GoalMapQuad;
        }
    }
}