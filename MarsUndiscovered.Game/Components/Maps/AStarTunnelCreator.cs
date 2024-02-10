using GoRogue.MapGeneration.TunnelCreators;
using GoRogue.Pathing;

using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Game.Components.Maps
{
    public class AStarTunnelCreator : ITunnelCreator
    {
        private readonly IGridView<bool> _allowedPoints;
        private readonly Distance _distanceMeasurement;
        private readonly bool _doubleWideDiagonal;

        /// <summary>
        /// Creates a tunnel between two points using the A* algorithm. Useful for when you have points on the map
        /// which you want to exclude from being turned into floors./>
        /// </summary>
        /// <param name="allowedPoints">The points on the map that the A* pathfinder is allowed to walk on.</param>
        /// <param name="distanceMeasurement">
        /// Distance measurement that will be passed into the A* pathfinder.
        /// </param>
        /// <param name="doubleWideDiagonal">When true, it will dig an extra square horizontally or vertically when digging a tunnel. example:
        /// false:   true:
        /// #######  #######
        /// ##.####  ##..###
        /// ###.###  ###..##
        /// ####.##  ####..#    
        /// #####.#  #####.#
        /// #######  #######
        ///
        /// This value is not used if using Distance.Manhattan.
        /// </param>
        public AStarTunnelCreator(IGridView<bool> allowedPoints, Distance distanceMeasurement,
            bool doubleWideDiagonal = false)
        {
            _allowedPoints = allowedPoints;
            _distanceMeasurement = distanceMeasurement;
            _doubleWideDiagonal = doubleWideDiagonal;
        }

        /// <summary>
        /// Implements the algorithm, creating the tunnel between the two points (ensuring there is a
        /// path of positions set to true between those two points).
        /// </summary>
        /// <param name="map">_grid to create the tunnel on.</param>
        /// <param name="tunnelStart">Start position to connect.</param>
        /// <param name="tunnelEnd">End position to connect.</param>
        /// <returns>An area containing all points that are part of the tunnel.  The area will be empty if the A* algorithm could
        /// not find a path between the two points.</returns>
        public Area CreateTunnel(ISettableGridView<bool> map, Point tunnelStart, Point tunnelEnd)
        {
            var area = new Area();

            var previous = Point.None;

            var aStar = new AStar(_allowedPoints, _distanceMeasurement);

            var path = aStar.ShortestPath(tunnelStart, tunnelEnd);

            // No path found
            if (path == null)
                return area;
            
            foreach (var pos in path.StepsWithStart)
            {
                area.Add(pos);

                if (_doubleWideDiagonal && !_distanceMeasurement.Equals(Distance.Manhattan) && previous != Point.None && pos.Y != previous.Y && pos.X != previous.X)
                {
                    var wideningPos = Point.None;

                    var direction = Direction.GetDirection(previous, pos);
                    
                    if (direction == Direction.UpLeft)
                    {
                        wideningPos = previous + (-1, 0);
                        if (!_allowedPoints.Contains(wideningPos) || !_allowedPoints[wideningPos])
                            wideningPos = previous + (0, -1);
                        if (!_allowedPoints.Contains(wideningPos) || !_allowedPoints[wideningPos])
                            // Cannot widen path in neither x nor y direction
                            return new Area();
                    }
                    else if (direction == Direction.UpRight)
                    {
                        wideningPos = previous + (1, 0);
                        if (!_allowedPoints.Contains(wideningPos) || !_allowedPoints[wideningPos])
                            wideningPos = previous + (0, -1);
                        if (!_allowedPoints.Contains(wideningPos) || !_allowedPoints[wideningPos])
                            // Cannot widen path in neither x nor y direction
                            return new Area();
                    }
                    else if (direction == Direction.DownLeft)
                    {
                        wideningPos = previous + (-1, 0);
                        if (!_allowedPoints.Contains(wideningPos) || !_allowedPoints[wideningPos])
                            wideningPos = previous + (0, 1);
                        if (!_allowedPoints.Contains(wideningPos) || !_allowedPoints[wideningPos])
                            // Cannot widen path in neither x nor y direction
                            return new Area();
                    }
                    else if (direction == Direction.DownRight)
                    {
                        wideningPos = previous + (1, 0);
                        if (!_allowedPoints.Contains(wideningPos) || !_allowedPoints[wideningPos])
                            wideningPos = previous + (0, 1);
                        if (!_allowedPoints.Contains(wideningPos) || !_allowedPoints[wideningPos])
                            // Cannot widen path in neither x nor y direction
                            return new Area();
                    }

                    area.Add(wideningPos);
                }

                previous = pos;
            }

            foreach (var point in area)
            {
                map[point] = true;
            }
            
            return area;
        }

        /// <summary>
        /// Implements the algorithm, creating the tunnel between the two points (ensuring there is a
        /// path of positions set to true between those two points).
        /// </summary>
        /// <param name="map">_grid to create the tunnel on.</param>
        /// <param name="startX">X-value of the start position to connect.</param>
        /// <param name="startY">Y-value of the start position to connect.</param>
        /// <param name="endX">X-value of the end position to connect.</param>
        /// <param name="endY">Y-value of the end position to connect.</param>
        /// <returns>An area containing all points that are part of the tunnel.  The area will be empty if the A* algorithm could
        /// not find a path between the two points.</returns>
        public Area CreateTunnel(ISettableGridView<bool> map, int startX, int startY, int endX, int endY)
            => CreateTunnel(map, new Point(startX, startY), new Point(endX, endY));
    }
}
