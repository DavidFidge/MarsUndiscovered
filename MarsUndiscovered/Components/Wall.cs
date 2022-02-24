using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using MarsUndiscovered.Components.SaveData;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Components
{
    public class Wall : Terrain, IMementoState<WallSaveData>
    {
        public Wall(IGameWorld gameWorld, uint id) : base(gameWorld, id, false, false)
        {
        }

        public IMemento<WallSaveData> GetSaveState()
        {
            var memento = new Memento<WallSaveData>(new WallSaveData());

            base.PopulateSaveState(memento.State);

            return memento;
        }

        public void SetLoadState(IMemento<WallSaveData> memento)
        {
            base.PopulateLoadState(memento.State);
        }
    }
}