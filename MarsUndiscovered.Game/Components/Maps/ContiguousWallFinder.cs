using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Game.Components.Maps;

public class ContiguousWallFinder
{
    public class Longest
    {
        public int Start { get; set; }
        public int End { get; set; }
        public int Index { get; set; }
        public int Length => Start == -1 || End == -1 ? -1 : End - Start;

        public OrthogonalEnumerator GetLine()
        {
            return Lines.GetOrthogonalLine(new Point(Start, Index), new Point(End, Index));
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

    public void Execute(ArrayView<bool> wallsFloors)
    {
        ExecuteX(wallsFloors);
        ExecuteY(wallsFloors);
    }

    public void ExecuteY(ArrayView<bool> wallsFloors)
    {
        AllYLongest = new List<Longest>(wallsFloors.Width);

        for (var x = 0; x < wallsFloors.Width; x++)
        {
            InitialiseY(wallsFloors[new Point(x, 0)]);

            for (var y = 0; y < wallsFloors.Height; y++)
            {
                var nextPoint = new Point(x, y);

                if (wallsFloors[nextPoint] != LastY)
                {
                    if (wallsFloors[nextPoint] == true)
                    {
                        YOff = y;

                        TryReplaceYLongest();
                    }
                    else
                    {
                        YOn = y;
                    }

                    LastY = !LastY;
                }
            }

            AllYLongest.Add(new Longest { Start = YOnLongest, End = YOffLongest, Index = x });
        }
    }

    public void ExecuteX(ArrayView<bool> wallsFloors)
    {
        AllXLongest = new List<Longest>(wallsFloors.Height);

        for (var y = 0; y < wallsFloors.Height; y++)
        {
            InitialiseX(wallsFloors[new Point(0, y)]);

            for (var x = 0; x < wallsFloors.Height; x++)
            {
                var nextPoint = new Point(x, y);

                if (wallsFloors[nextPoint] != LastX)
                {
                    if (wallsFloors[nextPoint] == true)
                    {
                        XOff = x;

                        TryReplaceXLongest();
                    }
                    else
                    {
                        XOn = x;
                    }

                    LastX = !LastX;
                }
            }

            AllXLongest.Add(new Longest { Start = XOnLongest, End = XOffLongest, Index = y });
        }
    }

    public Point LongestXYIntersect(int numberOfLinesToTest = 10)
    {
        var sortedXLongest = AllXLongest.OrderByDescending(l => l.Length);
        var sortedYLongest = AllYLongest.OrderByDescending(l => l.Length);

        // Don't test too many
        foreach(var xItem in sortedXLongest.Take(numberOfLinesToTest))
        {
            var yTest = sortedYLongest.Take(2);

            var xLine = xItem.GetLine();

            foreach (var yItem in yTest)
            {
                var yLine = yItem.GetLine();

                var intersect = yLine.Intersect(xLine).DefaultIfEmpty(Point.None).FirstOrDefault();

                if (intersect != Point.None)
                    return intersect;
            }
        }

        return Point.None;
    }
}
