using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Components
{
    public class SpawnMapExitParams : BaseSpawnGameObjectParams
    {
        public uint? DestinationMapExitId { get; set; }
        public Point LandingPosition { get; set; } = Point.None;
        public MapExitType MapExitType { get; set; }
        public MapExit Result { get; set; }
        public bool UseSeparation { get; set; }
    }

    public static class SpawnMapExitParamsFluentExtensions
    {
        public static SpawnMapExitParams WithMapExitType(this SpawnMapExitParams spawnMapExitParams, MapExitType mapExitType)
        {
            spawnMapExitParams.MapExitType = mapExitType;

            return spawnMapExitParams;
        }

        public static SpawnMapExitParams AtLandingPosition(this SpawnMapExitParams spawnMapExitParams, Point landingPosition)
        {
            spawnMapExitParams.LandingPosition = landingPosition;

            return spawnMapExitParams;
        }

        public static SpawnMapExitParams WithSeparationBetweenMapExitPoints(this SpawnMapExitParams spawnMapExitParams)
        {
            spawnMapExitParams.UseSeparation = true;
            return spawnMapExitParams;
        }
    }
}