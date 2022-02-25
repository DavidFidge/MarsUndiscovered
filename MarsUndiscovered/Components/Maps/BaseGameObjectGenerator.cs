using System;
using System.Linq;

using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Extensions;

using GoRogue.GameFramework;
using GoRogue.Random;

using SadRogue.Primitives;

using ShaiRandom.Generators;

namespace MarsUndiscovered.Components.Maps
{
    public abstract class BaseGameObjectGenerator : BaseComponent
    {
        protected Point GetPosition<T>(T spawnGameObjectParams, MarsMap map) where T : BaseSpawnGameObjectParams
        {
            if (spawnGameObjectParams.Position != null)
                return spawnGameObjectParams.Position.Value;

            if (spawnGameObjectParams.AvoidPosition != null)
                return map.RandomPositionAwayFrom(
                    spawnGameObjectParams.AvoidPosition.Value,
                    spawnGameObjectParams.AvoidPositionRange,
                    MapHelpers.EmptyPointOnFloor
                );

            return GlobalRandom.DefaultRNG.RandomPosition(map, MapHelpers.EmptyPointOnFloor);
        }

        protected Point GetWallPositionAdjacentToFloor<T>(T spawnGameObjectParams, MarsMap map) where T : BaseSpawnGameObjectParams
        {
            if (spawnGameObjectParams.Position != null)
                return spawnGameObjectParams.Position.Value;

            var validValues = map.Walls
                .Where(w => AdjacencyRule.EightWay.Neighbors(w.Position).Any(n => map.GetTerrainAt<Floor>(n) != null))
                .Where(w => map.GetObjectsAt(w.Position).Count() == 1)
                .ToHashSet();

            if (validValues.IsEmpty())
                throw new Exception("No valid empty wall positions that are adjacent to a floor tile were found.");

            if (spawnGameObjectParams.AvoidPosition != null)
            {
                validValues = validValues
                    .Where(w => MapExtensions.MinSeparationFrom(spawnGameObjectParams.AvoidPosition.Value, w.Position, spawnGameObjectParams.AvoidPositionRange))
                    .ToHashSet();
            }

            return GlobalRandom.DefaultRNG.RandomPosition(map, validValues);
        }
    }
}
