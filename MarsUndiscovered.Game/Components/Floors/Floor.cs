using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;
using MarsUndiscovered.Game.Components.SaveData;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components
{
    public class Floor : Terrain, IMementoState<FloorSaveData>
    {
        public override char AsciiCharacter => FloorType.AsciiCharacter;

        public FloorType FloorType { get; set; }

        public Floor(IGameWorld gameWorld, uint id) : base(gameWorld, id)
        {
        }

        public IMemento<FloorSaveData> GetSaveState()
        {
            var memento = new Memento<FloorSaveData>(new FloorSaveData());

            PopulateSaveState(memento.State);
            memento.State.FloorTypeName = FloorType.Name;

            return memento;
        }

        public void SetLoadState(IMemento<FloorSaveData> memento)
        {
            PopulateLoadState(memento.State);
            FloorType = FloorType.FloorTypes[memento.State.FloorTypeName];
        }
    }
}