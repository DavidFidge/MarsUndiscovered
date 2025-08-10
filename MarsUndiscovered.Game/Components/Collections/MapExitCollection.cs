using FrigidRogue.MonoGame.Core.Interfaces.Components;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.SaveData;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components
{
    public class MapExitCollection : GameObjectCollection<MapExit, MapExitSaveData>
    {
        public MapExitCollection(IGameObjectFactory gameObjectFactory) : base(gameObjectFactory)
        {
        }

        protected override void AfterCollectionLoaded(IGameWorld gameWorld, IList<IMemento<MapExitSaveData>> saveData)
        {
            foreach (var item in saveData)
            {
                var mapExit = this[item.State.Id];
                mapExit.Destination = this[item.State.DestinationId];
            }
        }

        public IEnumerable<MapExit> ForMap(MarsMap map)
        {
            return Values.Where(exit => exit.CurrentMap == map);
        }
    }
}
