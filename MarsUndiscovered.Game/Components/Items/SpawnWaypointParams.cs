namespace MarsUndiscovered.Game.Components
{
    public class SpawnWaypointParams : BaseSpawnGameObjectParams
    {
        public string WaypointName { get; set; }
        public Waypoint Result { get; set; }
    }

    public static class SpawnWaypointParamsFluentExtensions
    {
        public static SpawnWaypointParams WithWaypointName(this SpawnWaypointParams spawnWaypointParams, string waypointName)
        {
            spawnWaypointParams.WaypointName = waypointName;
            return spawnWaypointParams;
        }
    }
}