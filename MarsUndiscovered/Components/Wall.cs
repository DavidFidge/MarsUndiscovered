using AutoMapper;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using MarsUndiscovered.Components.SaveData;

namespace MarsUndiscovered.Components
{
    public class Wall : Terrain, IMementoState<WallSaveData>
    {
        public Wall() : base(false, false)
        {
        }

        public Wall(uint id) : base(false, false)
        {
        }

        public IMemento<WallSaveData> GetState(IMapper mapper)
        {
            return CreateWithAutoMapper<WallSaveData>(mapper);
        }

        public void SetState(IMemento<WallSaveData> state, IMapper mapper)
        {
            SetWithAutoMapper<WallSaveData>(state, mapper);
        }
    }
}