using AutoMapper;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using MarsUndiscovered.Components.SaveData;

namespace MarsUndiscovered.Components
{
    public class Monster : Actor, IMementoState<MonsterSaveData>
    {
        public Breed Breed { get; set; }

        public int Health { get; set; }

        public Monster() : base(1, true)
        {
        }

        public Monster(uint id) : base(1, true, idGenerator: () => id)
        {
        }

        public Monster WithBreed(Breed breed)
        {
            Breed = breed;
            Health = (int)(BaseHealth * breed.HealthModifier);

            return this;
        }

        public void SetState(IMemento<MonsterSaveData> state, IMapper mapper)
        {
            SetWithAutoMapper<MonsterSaveData>(state, mapper);
        }

        public IMemento<MonsterSaveData> GetState(IMapper mapper)
        {
            return CreateWithAutoMapper<MonsterSaveData>(mapper);
        }
    }
}