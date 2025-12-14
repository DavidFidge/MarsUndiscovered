namespace MarsUndiscovered.Game.Components
{
    public class SpawnWaypointParams : BaseSpawnGameObjectParams
    {
        public string Name { get; set; }
        public Waypoint Result { get; set; }
    }

    public static class SpawnWaypointParamsFluentExtensions
    {
        public static SpawnWaypointParams WithName(this SpawnWaypointParams spawnWaypointParams, string name)
        {
            spawnWaypointParams.Name = name;
            return spawnWaypointParams;
        }
    }
}