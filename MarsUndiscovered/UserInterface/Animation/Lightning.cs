using FrigidRogue.MonoGame.Core.Extensions;

using Microsoft.Xna.Framework;

using Point = SadRogue.Primitives.Point;

namespace MarsUndiscovered.UserInterface.Animation
{
    public class Lightning
    {
        public Lightning(IList<Point> path)
        {
            Path = path;

            From = FromPoint.ToVector();
            To = ToPoint.ToVector();
        }

        public Point FromPoint => Path.First();
        public Point ToPoint => Path.Last();

        public IList<Point> Path { get; }

        public Vector2 From { get; set; }
        public Vector2 To { get; set; }
    }
}
