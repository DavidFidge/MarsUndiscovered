using FrigidRogue.MonoGame.Core.Extensions;
using GoRogue.GameFramework;
using GoRogue.Random;
using SadRogue.Primitives;

namespace MarsUndiscovered.Components
{
    public abstract class BaseSpawnGameObjectParams
    {
        public Guid? MapId { get; set; }
        public Point? Position { get; set; }
        public Point? AvoidPosition { get; set; }
        public uint AvoidPositionRange { get; set; }
    }

    public static class BaseSpawnGameObjectParamsFluentExtensions
    {
        public static T AtPosition<T>(this T spawnParams, Point point) where T : BaseSpawnGameObjectParams
        {
            spawnParams.Position = point;

            CheckPositionAgainstAvoidPosition(spawnParams);

            return spawnParams;
        }
        
        public static T AtFreeSpotNextTo<T>(this T spawnParams, Map map, IList<Point> points) where T : BaseSpawnGameObjectParams
        {
            Point? chosenPoint = null;
            
            var randomPoints = points.ToList();

            for (var i = 0; i < points.Count; i++)
            {
                var randomPoint = randomPoints[GlobalRandom.DefaultRNG.NextInt(0, randomPoints.Count)];

                var neighbours = randomPoint
                    .Neighbours(map)
                    .Where(o => map.GetObjectsAt(o).Count() == 1 && map.GetObjectsAt(o).First() is Floor)
                    .ToList();

                if (neighbours.Count > 0)
                {
                    chosenPoint = neighbours[GlobalRandom.DefaultRNG.NextInt(0, neighbours.Count)];
                }
                else
                {
                    randomPoints.Remove(randomPoint);
                }
                
                if (chosenPoint != null)
                    break;
            }

            if (chosenPoint == null)
                throw new Exception("Could not find a free spot next to points passed in.");
            
            spawnParams.Position = chosenPoint;

            return spawnParams;
        }

        public static T OnMap<T>(this T spawnParams, Guid mapId) where T : BaseSpawnGameObjectParams
        {
            spawnParams.MapId = mapId;

            return spawnParams;
        }
        
        private static void CheckPositionAgainstAvoidPosition<T>(T spawnParams) where T : BaseSpawnGameObjectParams
        {
            if (spawnParams.AvoidPosition != null &&
                spawnParams.Position != null &&
                Distance.Chebyshev.Calculate(
                    spawnParams.AvoidPosition.Value,
                    spawnParams.Position.Value
                ) < spawnParams.AvoidPositionRange)
            {
                throw new Exception(
                    $"Position to spawn {spawnParams.Position} must be at least {spawnParams.AvoidPositionRange} units away from avoid position {spawnParams.AvoidPosition}"
                );
            }
        }

        public static T AvoidingPosition<T>(this T spawnParams, Point point, uint avoidPositionRange) where T : BaseSpawnGameObjectParams
        {
            spawnParams.AvoidPosition = point;
            spawnParams.AvoidPositionRange = avoidPositionRange;

            CheckPositionAgainstAvoidPosition(spawnParams);

            return spawnParams;
        }
    }
}