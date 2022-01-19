using System;

using GoRogue.GameFramework;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Extensions;
using MarsUndiscovered.Messages;

using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Components.Maps
{
    public class MapExitGenerator : BaseGameObjectGenerator, IMapExitGenerator
    {
        public MapExit SpawnMapExit(
            SpawnMapExitParams spawnMapExitParams,
            IGameObjectFactory gameObjectFactory,
            Map map,
            MapExitCollection mapExitCollection
        )
        {
            MapExit destinationMapExit = null;

            if (spawnMapExitParams.DestinationMapExitId != null)
                destinationMapExit =
                    (MapExit)gameObjectFactory.GameObjects[spawnMapExitParams.DestinationMapExitId.Value];

            var position = GetPosition(spawnMapExitParams, map);

            var landingPosition = spawnMapExitParams.LandingPosition;

            // TODO - this needs much improvement!
            if (landingPosition.Equals(Point.None))
            {
                landingPosition = position;

                foreach (var dir in AdjacencyRule.EightWay.DirectionsOfNeighbors())
                {
                    var candidateLandingPosition = landingPosition.Add(dir);
                    if (map.Bounds().Contains(candidateLandingPosition) &&
                        map.GetTerrainAt(candidateLandingPosition) is Floor)
                    {
                        landingPosition = candidateLandingPosition;
                        break;
                    }
                }

                if (landingPosition.Equals(position))
                    throw new Exception("No landing position found for map exit");
            }

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
    }
}
