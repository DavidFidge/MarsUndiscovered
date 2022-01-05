using AutoMapper;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using MarsUndiscovered.Components.SaveData;

namespace MarsUndiscovered.Components
{
    public class Floor : Terrain, IMementoState<FloorSaveData>
    {
        public Floor(uint id) : base(id)
        {
        }

        public IMemento<FloorSaveData> GetSaveState(IMapper mapper)
        {
            return CreateWithAutoMapper<FloorSaveData>(mapper);
        }

        public void SetLoadState(IMemento<FloorSaveData> memento, IMapper mapper)
        {
            SetWithAutoMapper<FloorSaveData>(memento, mapper);
        }
    }
}