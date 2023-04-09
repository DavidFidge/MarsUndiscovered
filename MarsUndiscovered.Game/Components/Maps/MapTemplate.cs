using System.Collections;
using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Components.Maps
{
    public class MapTemplate : IEnumerable<MapTemplateItem>
    {
        private readonly string[] _lines;
        public Rectangle Bounds { get; private set; }

        public MapTemplate(string[] lines) : this(lines, new Point(0, 0))
        {
        }
        
        public MapTemplate(string[] lines, int mapPositionX, int mapPositionY) : this(lines, new Point(mapPositionX, mapPositionY))
        {
        }
        
        public MapTemplate(string[] lines, Point mapPosition)
        {
            _lines = lines;
            var maxExtent = new Point(mapPosition.X + _lines[0].Length - 1, mapPosition.Y + _lines.Length - 1);
            
            Bounds = new Rectangle(
                mapPosition,
                maxExtent
            );
        }

        public char GetCharAt(Point point)
        {
            return GetCharAt(point.X, point.Y);
        }

        public char GetCharAt(int x, int y)
        {
            return _lines[y - Bounds.MinExtentY][x - Bounds.MinExtentX];
        }

        public IEnumerator<MapTemplateItem> GetEnumerator()
        {
            for (var y = Bounds.MinExtentY; y <= Bounds.MaxExtentY; y++)
            {
                for (var x = Bounds.MinExtentX; x <= Bounds.MaxExtentX; x++)
                {
                    yield return new MapTemplateItem(GetCharAt(x, y), new Point(x, y));
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
