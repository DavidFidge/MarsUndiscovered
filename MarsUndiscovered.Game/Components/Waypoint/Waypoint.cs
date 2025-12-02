using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components
{
    public class Waypoint : MarsGameObject, IMementoState<WaypointSaveData>
    {
        public string Name { get; set; }

        public Waypoint(IGameWorld gameWorld, uint id) : base(gameWorld, Constants.WaypointLayer, idGenerator: () => id)
        {
        }
        
        public IMemento<WaypointSaveData> GetSaveState()
        {
            var memento = new Memento<WaypointSaveData>(new WaypointSaveData());

            PopulateSaveState(memento.State);
            memento.State.Name = Name;

            return memento;
        }
        
        public void SetLoadState(IMemento<WaypointSaveData> memento)
        {
            PopulateLoadState(memento.State);
            Name = memento.State.Name;
        }

        public Waypoint WithName(string name)
        {
            Name = name;

            return this;
        }
    }
}