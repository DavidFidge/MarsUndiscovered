using AutoMapper;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using MarsUndiscovered.Components.SaveData;

namespace MarsUndiscovered.Components
{
    public class Player : Actor, IMementoState<PlayerSaveData>, ISaveable
    {
        public Player() : base(1, true)
        {
        }

        public Player(uint id) : base(1, true, idGenerator: () => id)
        {
        }

        public IMemento<PlayerSaveData> GetState(IMapper mapper)
        {
            return CreateWithAutoMapper<PlayerSaveData>(mapper);
        }

        public void SetState(IMemento<PlayerSaveData> state, IMapper mapper)
        {
            SetWithAutoMapper(state, mapper);
        }

        public void SaveGame(ISaveGameStore saveGameStore)
        {
            saveGameStore.SaveToStore(CreateWithAutoMapper<PlayerSaveData>(saveGameStore.Mapper));
        }

        public void LoadGame(ISaveGameStore saveGameStore)
        {
            var playerSaveData = saveGameStore.GetFromStore<PlayerSaveData>();
            SetState(playerSaveData, saveGameStore.Mapper);
        }
    }
}