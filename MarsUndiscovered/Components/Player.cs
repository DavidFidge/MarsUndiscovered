using AutoMapper;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using MarsUndiscovered.Components.SaveData;

namespace MarsUndiscovered.Components
{
    public class Player : Actor, IMementoState<PlayerSaveData>, ISaveable
    {
        public override string Name => "You";

        public Player() : base(1)
        {
        }

        public Player(uint id) : base(1, idGenerator: () => id)
        {
        }

        public IMemento<PlayerSaveData> GetSaveState(IMapper mapper)
        {
            return CreateWithAutoMapper<PlayerSaveData>(mapper);
        }

        public void SetLoadState(IMemento<PlayerSaveData> memento, IMapper mapper)
        {
            SetWithAutoMapper(memento, mapper);
        }

        public void SaveState(ISaveGameStore saveGameStore)
        {
            saveGameStore.SaveToStore(GetSaveState(saveGameStore.Mapper));
        }

        public void LoadState(ISaveGameStore saveGameStore)
        {
            var playerSaveData = saveGameStore.GetFromStore<PlayerSaveData>();
            SetLoadState(playerSaveData, saveGameStore.Mapper);
        }
    }
}