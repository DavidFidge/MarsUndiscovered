using System;
using SadRogue.Primitives;

namespace MarsUndiscovered.Components
{
    public class SpawnItemParams
    {
        public ItemType ItemType { get; set; }
        public Point? Position { get; set; }
        public Point? AvoidPosition { get; set; }
        public uint AvoidPositionRange { get; set; }
    }

    public static class SpawnItemParamsFluentExtensions
    {
        public static SpawnItemParams WithItemType(this SpawnItemParams spawnItemParams, ItemType itemType)
        {
            spawnItemParams.ItemType = itemType;
            return spawnItemParams;
        }

        public static SpawnItemParams WithItemType(this SpawnItemParams spawnItemParams, string breed)
        {
            spawnItemParams.ItemType = ItemType.GetItemType(breed);
            return spawnItemParams;
        }

        public static SpawnItemParams AtPosition(this SpawnItemParams spawnItemParams, Point point)
        {
            spawnItemParams.Position = point;

            CheckPositionAgainstAvoidPosition(spawnItemParams);

            return spawnItemParams;
        }

        private static void CheckPositionAgainstAvoidPosition(SpawnItemParams spawnItemParams)
        {
            if (spawnItemParams.AvoidPosition != null &&
                spawnItemParams.Position != null &&
                Distance.Chebyshev.Calculate(
                    spawnItemParams.AvoidPosition.Value,
                    spawnItemParams.Position.Value
                ) < spawnItemParams.AvoidPositionRange)
            {
                throw new Exception(
                    $"Position to spawn {spawnItemParams.Position} must be at least {spawnItemParams.AvoidPositionRange} units away from avoid position {spawnItemParams.AvoidPosition}"
                );
            }
        }

        public static SpawnItemParams AvoidingPosition(this SpawnItemParams spawnItemParams, Point point, uint avoidPositionRange)
        {
            spawnItemParams.AvoidPosition = point;
            spawnItemParams.AvoidPositionRange = avoidPositionRange;

            CheckPositionAgainstAvoidPosition(spawnItemParams);

            return spawnItemParams;
        }
    }
}