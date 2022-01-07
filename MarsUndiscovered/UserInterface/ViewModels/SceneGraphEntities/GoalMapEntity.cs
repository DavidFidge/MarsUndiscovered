using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Graphics.Quads;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using GoRogue.GameFramework;
using MarsUndiscovered.Interfaces;

using Microsoft.Xna.Framework;

using IDrawable = FrigidRogue.MonoGame.Core.Graphics.IDrawable;

using Point = SadRogue.Primitives.Point;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public class GoalMapEntity : Entity, IDrawable
    {
        public IGameProvider GameProvider { get; set; }
        public IAssets Assets { get; set; }
        public Point Point { get; set; }
        public int MaxHeight { get; set; }
        public GoalMapQuad GoalMapQuad { get; set; }
        public bool IsVisible { get; set; } = true;

        public string Text { get; set; }

        public void Draw(Matrix view, Matrix projection, Matrix world)
        {
            GoalMapQuad.Text = Text;
            GoalMapQuad.Draw(view, projection, world);
        }

        public void Initialize(IGameObject gameObject, string text)
        {
            Point = gameObject.Position;
            MaxHeight = gameObject.CurrentMap.Height;
            Transform.ChangeTranslation(new Vector3(Point.X * Graphics.Assets.TileQuadWidth, -Point.Y * Graphics.Assets.TileQuadHeight, 0));
            GoalMapQuad = Assets.GoalMapQuad;
            Text = text;
        }
    }
}