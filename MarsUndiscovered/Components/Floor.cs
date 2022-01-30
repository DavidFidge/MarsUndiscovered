using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using MarsUndiscovered.Components.SaveData;

namespace MarsUndiscovered.Components
{
    public class Floor : Terrain, IMementoState<FloorSaveData>
    {
        public Floor(uint id) : base(id)
        {
        }

        public IMemento<FloorSaveData> GetSaveState()
        {
            var memento = new Memento<FloorSaveData>(new FloorSaveData());

            base.PopulateSaveState(memento.State);

            return memento;
        }

        public void SetLoadState(IMemento<FloorSaveData> memento)
        {
            base.PopulateLoadState(memento.State);
        }
    }
}