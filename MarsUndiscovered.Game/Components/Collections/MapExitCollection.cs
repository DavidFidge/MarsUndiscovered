using FrigidRogue.MonoGame.Core.Interfaces.Components;

using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.SaveData;

namespace MarsUndiscovered.Game.Components
{
    public class MapExitCollection : GameObjectCollection<MapExit, MapExitSaveData>
    {
        private readonly IGameObjectFactory _gameObjectFactory;

        public MapExitCollection(IGameObjectFactory gameObjectFactory)
        {
            _gameObjectFactory = gameObjectFactory;
        }

        protected override MapExit Create(uint id)
        {
            return _gameObjectFactory.CreateMapExit(id);
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
