using System;
using SadRogue.Primitives;

namespace MarsUndiscovered.Components
{
    public class SpawnMonsterParams
    {
        public Breed Breed { get; set; }

        public Guid? MapId { get; set; }
        public Point? Position { get; set; }
        public Point? AvoidPosition { get; set; }
        public uint AvoidPositionRange { get; set; }
    }

    public static class SpawnMonsterParamsFluentExtensions
    {
        public static SpawnMonsterParams WithBreed(this SpawnMonsterParams spawnMonsterParams, Breed breed)
        {
            spawnMonsterParams.Breed = breed;
            return spawnMonsterParams;
        }

        public static SpawnMonsterParams WithBreed(this SpawnMonsterParams spawnMonsterParams, string breed)
        {
            spawnMonsterParams.Breed = Breed.GetBreed(breed);
            return spawnMonsterParams;
        }

        public static SpawnMonsterParams AtPosition(this SpawnMonsterParams spawnMonsterParams, Point point)
        {
            spawnMonsterParams.Position = point;

            CheckPositionAgainstAvoidPosition(spawnMonsterParams);

            return spawnMonsterParams;
        }

        public static SpawnMonsterParams OnMap(this SpawnMonsterParams spawnMonsterParams, Guid mapId)
        {
            spawnMonsterParams.MapId = mapId;

            return spawnMonsterParams;
        }

        private static void CheckPositionAgainstAvoidPosition(SpawnMonsterParams spawnMonsterParams)
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

        public static SpawnMonsterParams AvoidingPosition(this SpawnMonsterParams spawnMonsterParams, Point point, uint avoidPositionRange)
        {
            spawnMonsterParams.AvoidPosition = point;
            spawnMonsterParams.AvoidPositionRange = avoidPositionRange;

            CheckPositionAgainstAvoidPosition(spawnMonsterParams);

            return spawnMonsterParams;
        }
    }
}