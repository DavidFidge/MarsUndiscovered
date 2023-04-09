using FrigidRogue.MonoGame.Core.Extensions;

using GoRogue.GameFramework;
using GoRogue.Random;

using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Extensions;
using MarsUndiscovered.Game.ViewMessages;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

using ShaiRandom.Generators;

namespace MarsUndiscovered.Game.Components.Maps
{
    public class MapExitGenerator : BaseGameObjectGenerator, IMapExitGenerator
    {
        public MapExit SpawnMapExit(
            SpawnMapExitParams spawnMapExitParams,
            IGameObjectFactory gameObjectFactory,
            MarsMap map,
            MapExitCollection mapExitCollection
        )
        {
            MapExit destinationMapExit = null;

            if (spawnMapExitParams.DestinationMapExitId != null) 
                destinationMapExit =
                    (MapExit)gameObjectFactory.GameObjects[spawnMapExitParams.DestinationMapExitId.Value];

            var position = spawnMapExitParams.Position != null
                ? GetPosition(spawnMapExitParams, map)
                : GetPointOnWallAwayFromOtherExitPoints(map, mapExitCollection.Values);

            var landingPosition = spawnMapExitParams.LandingPosition;

            if (landingPosition.Equals(Point.None))
            {
                landingPosition = position;

                foreach (var dir in AdjacencyRule.Cardinals.DirectionsOfNeighbors())
                {
                    // Guaranteed to find a valid landing position due to GetPointOnWallAwayFromOtherExitPoints
                    var candidateLandingPosition = landingPosition.Add(dir);
                    if (map.Bounds().Contains(candidateLandingPosition) &&
                        map.GetTerrainAt(candidateLandingPosition) is Floor)
                    {
                        landingPosition = candidateLandingPosition;
                        break;
                    }
                }
            }

            map.CreateFloor(position, gameObjectFactory);

            var mapExit = gameObjectFactory
                .CreateMapExit()
                .PositionedAt(position)
                .WithDestination(destinationMapExit)
                .WithLandingPosition(landingPosition)
                .WithDirection(spawnMapExitParams.Direction)
                .AddToMap(map);

            mapExitCollection.Add(mapExit.ID, mapExit);

            Mediator.Publish(new MapTileChangedNotification(mapExit.Position));

            return mapExit;
        }

        private Point GetPointOnWallAwayFromOtherExitPoints(MarsMap map, IEnumerable<IGameObject> mapExitCollection)
        {
            var candidateWallPositions = map.Walls
                .Where(w => !w.IsDestroyed)
                .Select(w => w.Position)
                .Where(w => map.GetTerrainAround<Floor>(w, AdjacencyRule.Cardinals).Any())
                .ToList();

            if (!candidateWallPositions.Any())
            {
                // This would mean the whole map is a wall or there's no walls at all. Pick any empty point on the floor.
                candidateWallPositions.Add(GlobalRandom.DefaultRNG.RandomPosition(map, MapHelpers.EmptyPointOnFloor));
            }

            var otherMapExitPoints = mapExitCollection
                .Where(me => me.CurrentMap != null && me.CurrentMap.Equals(map))
                .Select(me => me.Position)
                .ToList();

            if (otherMapExitPoints.Any())
            {
                var splitPositionsByMagnitude =
                    candidateWallPositions.SplitIntoPointsBySumMagnitudeAgainstTargetPoints(otherMapExitPoints);

                candidateWallPositions = splitPositionsByMagnitude.Item2.Any()
                    ? splitPositionsByMagnitude.Item2.ToList()
                    : splitPositionsByMagnitude.Item1.ToList();
            }

            var position = candidateWallPositions[GlobalRandom.DefaultRNG.NextInt(0, candidateWallPositions.Count - 1)];
            return position;
        }
    }
}
