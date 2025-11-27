using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Game.Components.Maps;

public class ContiguousWallFinder
{
    public class Longest
    {
        public int Start { get; set; } = -1;
        public int End { get; set; } = -1;
        public int Index { get; set; } = -1;
        public int Length => Start == -1 || End == -1 ? -1 : End - Start;

        public OrthogonalEnumerator GetXLine()
        {
            return Lines.GetOrthogonalLine(new Point(Start, Index), new Point(End, Index));
        }

        public OrthogonalEnumerator GetYLine()
        {
            return Lines.GetOrthogonalLine(new Point(Index, Start), new Point(Index, End));
        }
    }

    public List<Longest> AllXLongest { get; set; }
    public List<Longest> AllYLongest { get; set; }
    public bool LastX { get; set; }
    public bool LastY { get; set; }

    public int XOn { get; set; } = -1;
    public int XOff { get; set; } = -1;
    public int YOn { get; set; } = -1;
    public int YOff { get; set; } = -1;

    public int XOnLongest { get; set; } = -1;
    public int XOffLongest { get; set; } = -1;
    public int YOnLongest { get; set; } = -1;
    public int YOffLongest { get; set; } = -1;

    public int Width { get; set; } = -1;
    public int Height { get; set; } = -1;

    public Rectangle Region { get; set; }

    public void TryReplaceXLongest()
    {
        if (XOn == -1 || XOff == -1)
            return;

        if (XOnLongest == -1 || XOffLongest == -1
            || XOffLongest - XOnLongest < XOff - XOn)
        {
            XOnLongest = XOn;
            XOffLongest = XOff;
        }
    }

    public void TryReplaceYLongest()
    {
        if (YOn == -1 || YOff == -1)
            return;

        if (YOnLongest == -1 || YOffLongest == -1
            || YOffLongest - YOnLongest < YOff - YOn)
        {
            YOnLongest = YOn;
            YOffLongest = YOff;
        }
    }

    public void InitialiseX(bool startingValue)
    {
        LastX = startingValue;
        XOn = startingValue == false ? 0 : -1;
        XOff = -1;
        XOnLongest = -1;
        XOffLongest = -1;
    }

    public void InitialiseY(bool startingValue)
    {
        LastY = startingValue;
        YOn = startingValue == false ? 0 : -1;
        YOff = -1;
        YOnLongest = -1;
        YOffLongest = -1;
    }

    public void Execute(ArrayView<bool> wallsFloors, Rectangle? region)
    {
        if (region == null)
            Region = wallsFloors.Bounds();
        else
            Region = region.Value;

        ExecuteX(wallsFloors);
        ExecuteY(wallsFloors);
    }

    public void ExecuteY(ArrayView<bool> wallsFloors)
    {
        Width = wallsFloors.Width;
        Height = wallsFloors.Height;

        AllYLongest = new List<Longest>(wallsFloors.Width);

        for (var x = Region.MinExtentX; x < Region.MaxExtentX; x++)
        {
            InitialiseY(wallsFloors[new Point(x, Region.MinExtentY)]);

            for (var y = Region.MinExtentY + 1; y < Region.MaxExtentY; y++)
            {
                var nextPoint = new Point(x, y);

                if (wallsFloors[nextPoint] != LastY)
                {
                    if (wallsFloors[nextPoint] == true)
                    {
                        YOff = y - 1;

                        TryReplaceYLongest();
                    }
                    else
                    {
                        YOn = y;
                    }

                    LastY = !LastY;
                }
                if (y == Region.MaxExtentY - 1 && LastY == false)
                {
                    YOff = y;

                    TryReplaceYLongest();
                }
            }

            AllYLongest.Add(new Longest { Start = YOnLongest, End = YOffLongest, Index = x });
        }
    }

    public void ExecuteX(ArrayView<bool> wallsFloors)
    {
        Width = wallsFloors.Width;
        Height = wallsFloors.Height;

        AllXLongest = new List<Longest>(wallsFloors.Height);

        for (var y = Region.MinExtentY; y < Region.MaxExtentY; y++)
        {
            InitialiseX(wallsFloors[new Point(Region.MinExtentX, y)]);

            for (var x = Region.MinExtentX + 1; x < Region.MaxExtentX; x++)
            {
                var nextPoint = new Point(x, y);

                if (wallsFloors[nextPoint] != LastX)
                {
                    if (wallsFloors[nextPoint] == true)
                    {
                        XOff = x - 1;

                        TryReplaceXLongest();
                    }
                    else
                    {
                        XOn = x;
                    }

                    LastX = !LastX;
                }

                if (x == Region.MaxExtentX - 1 && LastX == false)
                {
                    XOff = x;

                    TryReplaceXLongest();
                }
            }

            AllXLongest.Add(new Longest { Start = XOnLongest, End = XOffLongest, Index = y });
        }
    }

    public Point LongestXYIntersect(int numberOfLinesToTest = 10)
    {
        var sortedXLongest = AllXLongest
            .OrderByDescending(l => l.Length)
            // prefer lengths closer to the middle of the region
            .ThenBy(l => l.Index == -1 ? 0 : Math.Abs((Region.Height / 2) - l.Index));

        var sortedYLongest = AllYLongest
            .OrderByDescending(l => l.Length)
            .ThenBy(l => l.Index == -1 ? 0 : Math.Abs((Region.Width / 2) - l.Index));

        // Don't test too many
        foreach (var xItem in sortedXLongest.Take(numberOfLinesToTest))
        {
            var yTest = sortedYLongest.Take(2);

            var xLine = xItem.GetXLine();

            foreach (var yItem in yTest)
            {
                var yLine = yItem.GetYLine();

                var intersect = yLine.Intersect(xLine).DefaultIfEmpty(Point.None).FirstOrDefault();

                if (intersect != Point.None)
                    return intersect;
            }
        }

        return Point.None;
    }
}
