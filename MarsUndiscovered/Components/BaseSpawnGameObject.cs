using System;
using SadRogue.Primitives;

namespace MarsUndiscovered.Components
{
    public abstract class BaseSpawnGameObject
    {
        public Guid? MapId { get; set; }
        public Point? Position { get; set; }
        public Point? AvoidPosition { get; set; }
        public uint AvoidPositionRange { get; set; }
    }

    public static class BaseSpawnGameObjectFluentExtensions
    {
        public static T AtPosition<T>(this T spawnMonsterParams, Point point) where T : BaseSpawnGameObject
        {
            spawnMonsterParams.Position = point;

            CheckPositionAgainstAvoidPosition(spawnMonsterParams);

            return spawnMonsterParams;
        }

        public static T OnMap<T>(this T spawnMonsterParams, Guid mapId) where T : BaseSpawnGameObject
        {
            spawnMonsterParams.MapId = mapId;

            return spawnMonsterParams;
        }

        private static void CheckPositionAgainstAvoidPosition<T>(T spawnMonsterParams) where T : BaseSpawnGameObject
        {
            if (spawnMonsterParams.AvoidPosition != null &&
                spawnMonsterParams.Position != null &&
                Distance.Chebyshev.Calculate(
                    spawnMonsterParams.AvoidPosition.Value,
                    spawnMonsterParams.Position.Value
                ) < spawnMonsterParams.AvoidPositionRange)
            {
                throw new Exception(
                    $"Position to spawn {spawnMonsterParams.Position} must be at least {spawnMonsterParams.AvoidPositionRange} units away from avoid position {spawnMonsterParams.AvoidPosition}"
                );
            }
        }

        public static T AvoidingPosition<T>(this T spawnMonsterParams, Point point, uint avoidPositionRange) where T : BaseSpawnGameObject
        {
            spawnMonsterParams.AvoidPosition = point;
            spawnMonsterParams.AvoidPositionRange = avoidPositionRange;

            CheckPositionAgainstAvoidPosition(spawnMonsterParams);

            return spawnMonsterParams;
        }
    }
}