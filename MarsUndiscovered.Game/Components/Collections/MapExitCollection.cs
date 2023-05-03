using FrigidRogue.MonoGame.Core.Interfaces.Components;

using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.SaveData;

namespace MarsUndiscovered.Game.Components
{
    public class MapExitCollection : GameObjectCollection<MapExit, MapExitSaveData>
    {
        public MapExitCollection(IGameObjectFactory gameObjectFactory) : base(gameObjectFactory)
        {
        }

        protected override void AfterCollectionLoaded(IList<IMemento<MapExitSaveData>> saveData)
        {
            foreach (var item in saveData)
            {
                var mapExit = this[item.State.Id];
                var mapExitDestination = this[item.State.DestinationId];
                mapExit.Destination = mapExitDestination;
            }
        }
    }
}
