using System;
using GoRogue.MapGeneration.TunnelCreators;
using GoRogue.Pathing;

using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Game.Components.Maps
{
    public class AStarTunnelCreator : ITunnelCreator
    {
        private readonly ISettableGridView<bool> _allowedPoints;
        private readonly Distance _distanceMeasurement = Distance.Euclidean;
        public Distance DistanceMeasurement => _distanceMeasurement;
        
        private readonly bool _doubleWideDiagonal;

        /// <summary>
        /// Constructor. Takes the distance measurement that the A* pathfinder will use. />
        /// </summary>
        /// <param name="allowedPoints">The points on the map that the A* pathfinder is allowed to walk on.</param>
        /// <param name="distanceMeasurement">
        /// Distance measurement that will be passed into the A* pathfinder.
        /// </param>
        /// <param name="doubleWideDiagonal">Whether or not to create diagonals directly (false) or add extra points to turn it into a stepping pattern, which generally make it look nicer (true)</param>
        public AStarTunnelCreator(ISettableGridView<bool> allowedPoints, Distance distanceMeasurement, bool doubleWideDiagonal = true)
        {
            _allowedPoints = allowedPoints;
            _distanceMeasurement = distanceMeasurement;
            _doubleWideDiagonal = doubleWideDiagonal;
        }

        /// <inheritdoc />
        public Area CreateTunnel(ISettableGridView<bool> map, Point start, Point end)
        {
            var area = new Area();

            var previous = Point.None;

            var astar = new AStar(_allowedPoints, _distanceMeasurement);

            var path = astar.ShortestPath(start, end);

            if (path == null)
                return area;
            
            // Note - the path may still be rejected if using double wide
            // diagonals as the extra point may conflict with the mask
            foreach (var pos in path.StepsWithStart)
            {
                area.Add(pos);
                map[pos] = true;

                if (_doubleWideDiagonal && previous != Point.None && pos.Y != previous.Y && pos.X != previous.X)
                {
                    Point wideningPos = Point.None;

                    var direction = Direction.GetDirection(previous, pos);
                    
                    if (direction == Direction.UpLeft)
                    {
                        wideningPos = previous + (-1, 0);
                        if (!_allowedPoints[wideningPos])
                            wideningPos = previous + (0, -1);
                        if (!_allowedPoints[wideningPos])
                            return new Area();
                    }
                    else if (direction == Direction.UpRight)
                    {
                        wideningPos = previous + (1, 0);
                        if (!_allowedPoints[wideningPos])
                            wideningPos = previous + (0, -1);
                        if (!_allowedPoints[wideningPos])
                            return new Area();
                    }
                    else if (direction == Direction.DownLeft)
                    {
                        wideningPos = previous + (-1, 0);
                        if (!_allowedPoints[wideningPos])
                            wideningPos = previous + (0, 1);
                        if (!_allowedPoints[wideningPos])
                            return new Area();
                    }
                    else if (direction == Direction.DownRight)
                    {
                        wideningPos = previous + (1, 0);
                        if (!_allowedPoints[wideningPos])
                            wideningPos = previous + (0, 1);
                        if (!_allowedPoints[wideningPos])
                            return new Area();
                    }

                    area.Add(wideningPos);
                    map[wideningPos] = true;
                }

                previous = pos;
            }

            return area;
        }

        /// <inheritdoc />
        public Area CreateTunnel(ISettableGridView<bool> _allowedPoints, int startX, int startY, int endX, int endY)
            => CreateTunnel(_allowedPoints, new Point(startX, startY), new Point(endX, endY));
    }
}
