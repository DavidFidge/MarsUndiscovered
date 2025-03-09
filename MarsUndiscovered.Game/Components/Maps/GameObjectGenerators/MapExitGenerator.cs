using FrigidRogue.MonoGame.Core.Components.MapPointChoiceRules;
using FrigidRogue.MonoGame.Core.Extensions;
using GoRogue.GameFramework;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.Maps.MapPointChoiceRules;
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
            GameWorld gameWorld
        )
        {
            var maps = gameWorld.Maps;
            var mapExitCollection = gameWorld.MapExits;
            var gameObjectFactory = gameWorld.GameObjectFactory;
            
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
                .WithType(spawnMapExitParams.MapExitType)
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
        
        private void SpawnMapExitWithoutDefaultRules(
            SpawnMapExitParams spawnMapExitParams,
            GameWorld gameWorld
            )
        {
            spawnMapExitParams.MapPointChoiceRules.Add(new EmptyFloorRule());
            SpawnMapExit(spawnMapExitParams, gameWorld);
        }
        
        public void CreateMapEdgeExits(GameWorld gameWorld,
            MapExitType mapExitType,
            MarsMap map,
            MarsMap linkMap = null)
        {
            var mapExits = new List<MapExit>();

            if (mapExitType == MapExitType.MapExitNorth)
            {
                for (var x = 0; x < map.Width; x++)
                {
                    var spawnMapExitParams = new SpawnMapExitParams()
                        .OnMap(map.Id)
                        .AtPosition(new Point(x, 0))
                        .WithMapExitType(MapExitType.MapExitNorth);

                    spawnMapExitParams.MapPointChoiceRules.Add(new WallAdjacentToFloorRule());
                    SpawnMapExitWithoutDefaultRules(spawnMapExitParams, gameWorld);
                    mapExits.Add(spawnMapExitParams.Result);
                }
            }
            else if (mapExitType == MapExitType.MapExitSouth)
            {
                for (var x = 0; x < map.Width; x++)
                {
                    var spawnMapExitParams = new SpawnMapExitParams()
                        .OnMap(map.Id)
                        .AtPosition(new Point(x, map.Height - 1))
                        .WithMapExitType(MapExitType.MapExitSouth);

                    spawnMapExitParams.MapPointChoiceRules.Add(new WallAdjacentToFloorRule());
                    SpawnMapExitWithoutDefaultRules(spawnMapExitParams, gameWorld);
                    mapExits.Add(spawnMapExitParams.Result);
                }
            }
            else if (mapExitType == MapExitType.MapExitEast)
            {
                for (var y = 0; y < map.Height; y++)
                {
                    var spawnMapExitParams = new SpawnMapExitParams()
                        .OnMap(map.Id)
                        .AtPosition(new Point(map.Width - 1, y))
                        .WithMapExitType(MapExitType.MapExitEast);

                    spawnMapExitParams.MapPointChoiceRules.Add(new WallAdjacentToFloorRule());
                    SpawnMapExitWithoutDefaultRules(spawnMapExitParams, gameWorld);
                    mapExits.Add(spawnMapExitParams.Result);
                }
            }
            else if (mapExitType == MapExitType.MapExitWest)
            {
                for (var y = 0; y < map.Height; y++)
                {
                    var spawnMapExitParams = new SpawnMapExitParams()
                        .OnMap(map.Id)
                        .AtPosition(new Point(0, y))
                        .WithMapExitType(MapExitType.MapExitWest);

                    spawnMapExitParams.MapPointChoiceRules.Add(new WallAdjacentToFloorRule());
                    SpawnMapExitWithoutDefaultRules(spawnMapExitParams, gameWorld);
                    mapExits.Add(spawnMapExitParams.Result);
                }
            }

            if (linkMap != null)
            {
                MapExitType mapExitTypeLink = null;

                if (mapExitType == MapExitType.MapExitNorth)
                {
                    mapExitTypeLink = MapExitType.MapExitSouth;
                }
                else if (mapExitType == MapExitType.MapExitSouth)
                {
                    mapExitTypeLink = MapExitType.MapExitNorth;
                }
                else if (mapExitType == MapExitType.MapExitEast)
                {
                    mapExitTypeLink = MapExitType.MapExitWest;
                }
                else if (mapExitType == MapExitType.MapExitWest)
                {
                    mapExitTypeLink = MapExitType.MapExitEast;
                }

                var previousMapExits = gameWorld.MapExits.Values.Where(me =>
                    Equals(me.CurrentMap, linkMap) && me.MapExitType == mapExitTypeLink);

                for (var i = 0; i < mapExits.Count; i++)
                {
                    MapExit currentMapExit = mapExits.ElementAt(i);
                    MapExit previousMapExit = null;

                    if (mapExitType == MapExitType.MapExitNorth)
                    {
                        previousMapExit =
                            previousMapExits.FirstOrDefault(m => m.Position == new Point(i, m.CurrentMap.Height - 1));
                    }
                    else if (mapExitType == MapExitType.MapExitSouth)
                    {
                        previousMapExit =
                            previousMapExits.FirstOrDefault(m => m.Position == new Point(i, 0));
                    }
                    else if (mapExitType == MapExitType.MapExitEast)
                    {
                        previousMapExit =
                            previousMapExits.FirstOrDefault(m => m.Position == new Point(0, i));
                    }
                    else if (mapExitType == MapExitType.MapExitWest)
                    {
                        previousMapExit =
                            previousMapExits.FirstOrDefault(m => m.Position == new Point(m.CurrentMap.Width - 1, i));
                    }

                    if (previousMapExit == null)
                        throw new Exception("Could not find link map");
                    
                    previousMapExit.Destination = currentMapExit;
                    currentMapExit.Destination = previousMapExit;
                }
            }
        }
    }
}
