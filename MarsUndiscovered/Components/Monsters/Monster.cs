using AutoMapper;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using MarsUndiscovered.Components.SaveData;

namespace MarsUndiscovered.Components
{
    public class Monster : Actor, IMementoState<MonsterSaveData>
    {
        public Breed Breed { get; set; }
        public override string Name => Breed.Name;
        public override string Description => Breed.Description;

        public override int MaxHealth { get; protected set; }
        public override Attack BasicAttack => Breed.BasicAttack;

        public Monster(uint id) : base(1, idGenerator: () => id)
        {
        }

        public Monster WithBreed(Breed breed)
        {
            Breed = breed;
            MaxHealth = (int)(BaseHealth * Breed.HealthModifier);
            Health = MaxHealth;

            return this;
        }

        public void SetLoadState(IMemento<MonsterSaveData> memento, IMapper mapper)
        {
            SetWithAutoMapper(memento, mapper);
            Breed = Breed.GetBreed(memento.State.BreedName);
        }

        public IMemento<MonsterSaveData> GetSaveState(IMapper mapper)
        {
            return CreateWithAutoMapper<MonsterSaveData>(mapper);
        }
    }
}