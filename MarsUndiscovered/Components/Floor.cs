using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using MarsUndiscovered.Components.SaveData;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Components
{
    public class Floor : Terrain, IMementoState<FloorSaveData>
    {
        public Floor(IGameWorld gameWorld, uint id) : base(gameWorld, id)
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