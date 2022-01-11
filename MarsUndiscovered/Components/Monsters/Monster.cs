using System.Text;

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
        public override Attack BasicAttack => Breed.BasicAttack;

        public string GetInformation(Player player)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(Name);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(Description);
            stringBuilder.AppendLine();

            var percentMaxDamage = Breed.BasicAttack.DamageRange.Max * 100 / player.MaxHealth;
            var percentMinDamage = Breed.BasicAttack.DamageRange.Min * 100 / player.MaxHealth;
            var defeatTurns = player.Health / Breed.BasicAttack.DamageRange.Max;

            var percentText = percentMinDamage != percentMaxDamage
                ? $"between {percentMinDamage}-{percentMaxDamage}%"
                : $"{percentMinDamage}%";

            stringBuilder.AppendLine(
                $"{NameSpecificArticleUpperCase} can hit you for {percentText} of your maximum health and, at worst, could defeat you in {defeatTurns} hits."
            );

            return stringBuilder.ToString();
        }

        public Monster(uint id) : base(idGenerator: () => id)
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