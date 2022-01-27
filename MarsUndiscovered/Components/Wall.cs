using AutoMapper;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using MarsUndiscovered.Components.SaveData;

namespace MarsUndiscovered.Components
{
    public class Wall : Terrain, IMementoState<WallSaveData>
    {
        public Wall(uint id) : base(id, false, false)
        {
        }

        public IMemento<WallSaveData> GetSaveState()
        {
            var memento = new Memento<WallSaveData>();

            base.PopulateSaveState(memento.State);

            return memento;
        }

        public void SetLoadState(IMemento<WallSaveData> memento)
        {
            base.PopulateLoadState(memento.State);
        }
    }
}