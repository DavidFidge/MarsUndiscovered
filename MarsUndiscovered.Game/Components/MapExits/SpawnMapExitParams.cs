using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Components
{
    public class SpawnMapExitParams : BaseSpawnGameObjectParams
    {
        public uint? DestinationMapExitId { get; set; }
        public Point LandingPosition { get; set; } = Point.None;
        public Direction Direction { get; set; }
    }

    public static class SpawnMapExitParamsFluentExtensions
    {
        public static SpawnMapExitParams ToMapExit(this SpawnMapExitParams spawnMapExitParams, uint mapExitId)
        {
            spawnMapExitParams.DestinationMapExitId = mapExitId;

            return spawnMapExitParams;
        }

        public static SpawnMapExitParams WithDirection(this SpawnMapExitParams spawnMapExitParams, Direction direction)
        {
            spawnMapExitParams.Direction = direction;

            return spawnMapExitParams;
        }

        public static SpawnMapExitParams AtLandingPosition(this SpawnMapExitParams spawnMapExitParams, Point landingPosition)
        {
            spawnMapExitParams.LandingPosition = landingPosition;

            return spawnMapExitParams;
        }
    }
}