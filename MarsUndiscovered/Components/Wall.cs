using AutoMapper;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using MarsUndiscovered.Components.SaveData;

namespace MarsUndiscovered.Components
{
    public class Wall : Terrain, IMementoState<WallSaveData>
    {
        public Wall(uint id) : base(id, false, false)
        {
        }

        public IMemento<WallSaveData> GetSaveState(IMapper mapper)
        {
            return CreateWithAutoMapper<WallSaveData>(mapper);
        }

        public void SetLoadState(IMemento<WallSaveData> memento, IMapper mapper)
        {
            SetWithAutoMapper<WallSaveData>(memento, mapper);
        }
    }
}