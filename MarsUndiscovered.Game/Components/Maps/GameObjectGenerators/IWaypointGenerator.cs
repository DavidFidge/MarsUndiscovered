using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components.Maps
{
    public interface IWaypointGenerator
    {
        void SpawnWaypoint(SpawnWaypointParams spawnWaypointParams, IGameWorld gameWorld);
    }
}