using FrigidRogue.MonoGame.Core.Components.MapPointChoiceRules;
using FrigidRogue.MonoGame.Core.Extensions;
using GoRogue.GameFramework;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Extensions;
using MarsUndiscovered.Game.ViewMessages;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Game.Components.Maps
{
    public class MapExitGenerator : BaseGameObjectGenerator, IMapExitGenerator
    {
        public void SpawnMapExit(
            SpawnMapExitParams spawnMapExitParams,
            IGameObjectFactory gameObjectFactory,
            MapCollection maps,
            MapExitCollection mapExitCollection
        )
        {
            spawnMapExitParams.Result = null;
            var map = maps.Single(m => m.Id == spawnMapExitParams.MapId);
            spawnMapExitParams.AssignMap(map);
            
            MapExit destinationMapExit = null;

            if (spawnMapExitParams.DestinationMapExitId != null)
                destinationMapExit = mapExitCollection[spawnMapExitParams.DestinationMapExitId.Value];

            if (spawnMapExitParams.UseSeparation)
            {
                var separationRule = GetPointsAwayFromOtherExitPoints(map, mapExitCollection.Values);
                spawnMapExitParams.MapPointChoiceRules.Add(separationRule);
            }
            
            var position = spawnMapExitParams.Position == Point.None
                ? GetPosition(spawnMapExitParams, map)
                : spawnMapExitParams.Position;
                
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

            map.CreateFloor(FloorType.BlankFloor, position, gameObjectFactory);

            var mapExit = gameObjectFactory
                .CreateGameObject<MapExit>()
                .PositionedAt(position)
                .WithDestination(destinationMapExit)
                .WithLandingPosition(landingPosition)
                .WithDirection(spawnMapExitParams.Direction)
                .AddToMap(map);

            mapExitCollection.Add(mapExit.ID, mapExit);

            Mediator.Publish(new MapTileChangedNotification(mapExit.Position));

            spawnMapExitParams.Result = mapExit;
        }

        public RestrictedSetRule GetPointsAwayFromOtherExitPoints(MarsMap map,
            IEnumerable<IGameObject> mapExitCollection)
        {
            var otherMapExitPoints = mapExitCollection
                .Where(me => me.CurrentMap != null && me.CurrentMap.Equals(map))
                .Select(me => me.Position)
                .ToList();

            var candidatePoints = map.Positions().ToList();

            if (otherMapExitPoints.Any())
            {
                var splitPositionsByMagnitude =
                    candidatePoints.SplitIntoPointsBySumMagnitudeAgainstTargetPoints(otherMapExitPoints,
                        splittingPoint: 0.75f);

                candidatePoints = splitPositionsByMagnitude.Item2.Any()
                    ? splitPositionsByMagnitude.Item2.ToList()
                    : splitPositionsByMagnitude.Item1.ToList();
            }

            return new RestrictedSetRule(candidatePoints);
        }
    }
}
