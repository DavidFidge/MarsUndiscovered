using System;
using GoRogue.MapGeneration.TunnelCreators;
using GoRogue.Pathing;

using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Game.Components.Maps
{
    public class AStarTunnelCreator : ITunnelCreator
    {
        private readonly Distance _distanceMeasurement = Distance.Euclidean;
        public Distance DistanceMeasurement => _distanceMeasurement;
        
        private readonly bool _doubleWideDiagonal;
        
        /// <summary>
        /// Constructor. Takes the distance calculation to use, which determines whether <see cref="SadRogue.Primitives.Lines.Algorithm.Orthogonal" />
        /// or <see cref="SadRogue.Primitives.Lines.Algorithm.Bresenham" /> is used to create the tunnel.
        /// </summary>
        /// <param name="adjacencyRule">
        /// Method of adjacency to respect when creating tunnels. Cannot be diagonal.
        /// </param>
        /// <param name="doubleWideDiagonal">Whether or not to create diagonals directly (false) or add extra points to turn it into a stepping pattern, which generally make it look nicer (true)</param>
        public AStarTunnelCreator(Distance distanceMeasurement, bool doubleWideDiagonal = true)
        {
            _distanceMeasurement = distanceMeasurement;
            _doubleWideDiagonal = doubleWideDiagonal;
        }

        /// <inheritdoc />
        public Area CreateTunnel(ISettableGridView<bool> allowedPoints, Point start, Point end)
        {
            var area = new Area();

            var previous = Point.None;

            var astar = new AStar(allowedPoints, _distanceMeasurement);

            var path = astar.ShortestPath(start, end);

            if (path == null)
                return area;
            
            // Note - the path may still be rejected if using double wide
            // diagonals as the extra point may conflict with the mask
            foreach (var pos in path.StepsWithStart)
            {
                area.Add(pos);

                if (_doubleWideDiagonal && previous != Point.None && pos.Y != previous.Y && pos.X != previous.X)
                {
                    Point wideningPos = Point.None;

                    var direction = Direction.GetDirection(previous, pos);
                    
                    if (direction == Direction.UpLeft)
                    {
                        wideningPos = previous + (-1, 0);
                        if (!allowedPoints[wideningPos])
                            wideningPos = previous + (0, -1);
                        if (!allowedPoints[wideningPos])
                            return new Area();
                    }
                    else if (direction == Direction.UpRight)
                    {
                        wideningPos = previous + (1, 0);
                        if (!allowedPoints[wideningPos])
                            wideningPos = previous + (0, -1);
                        if (!allowedPoints[wideningPos])
                            return new Area();
                    }
                    else if (direction == Direction.DownLeft)
                    {
                        wideningPos = previous + (-1, 0);
                        if (!allowedPoints[wideningPos])
                            wideningPos = previous + (0, 1);
                        if (!allowedPoints[wideningPos])
                            return new Area();
                    }
                    else if (direction == Direction.DownRight)
                    {
                        wideningPos = previous + (1, 0);
                        if (!allowedPoints[wideningPos])
                            wideningPos = previous + (0, 1);
                        if (!allowedPoints[wideningPos])
                            return new Area();
                    }

                    area.Add(wideningPos);
                }

                previous = pos;
            }

            return area;
        }

        /// <inheritdoc />
        public Area CreateTunnel(ISettableGridView<bool> allowedPoints, int startX, int startY, int endX, int endY)
            => CreateTunnel(allowedPoints, new Point(startX, startY), new Point(endX, endY));
    }
}
