using AutoMapper;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using MarsUndiscovered.Components.SaveData;

namespace MarsUndiscovered.Components
{
    public class Floor : Terrain, IMementoState<FloorSaveData>
    {
        public Floor()
        {
        }

        public Floor(uint id) : base(id)
        {
        }

        public IMemento<FloorSaveData> GetState(IMapper mapper)
        {
            return CreateWithAutoMapper<FloorSaveData>(mapper);
        }

        public void SetState(IMemento<FloorSaveData> state, IMapper mapper)
        {
            SetWithAutoMapper<FloorSaveData>(state, mapper);
        }
    }
}