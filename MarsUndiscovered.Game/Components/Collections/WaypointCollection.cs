using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.SaveData;

namespace MarsUndiscovered.Game.Components
{
    public class WaypointCollection : GameObjectCollection<Waypoint, WaypointSaveData>
    {
        public WaypointCollection(IGameObjectFactory gameObjectFactory) : base(gameObjectFactory)
        {
        }
    }
}