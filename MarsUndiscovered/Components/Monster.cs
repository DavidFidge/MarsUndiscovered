using System.Text;

using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using MarsUndiscovered.Components.SaveData;
using MarsUndiscovered.Extensions;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Components
{
    public class Monster : Actor, IMementoState<MonsterSaveData>
    {
        public Breed Breed { get; set; }
        public override string Name => Breed.Name;
        public override string Description => Breed.Description;
        public override Attack BasicAttack => Breed.BasicAttack;
        public override LightningAttack LightningAttack => Breed.LightningAttack;

        public MonsterGoal MonsterGoal { get; set; }

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

        public Monster(IGameWorld gameWorld, uint id) : base(gameWorld, id)
        {
            MonsterGoal = new MonsterGoal(this);
        }

        public Monster WithBreed(Breed breed)
        {
            Breed = breed;
            MaxHealth = (int)(BaseHealth * Breed.HealthModifier);
            Health = MaxHealth;

            return this;
        }

        public Monster AddToMap(MarsMap marsMap)
        {
            MarsGameObjectFluentExtensions.AddToMap(this, marsMap);
            MonsterGoal.ChangeMap();

            return this;
        }

        public void SetLoadState(IMemento<MonsterSaveData> memento)
        {
            PopulateLoadState(memento.State);
            Breed = Breed.Breeds[memento.State.BreedName];

            MonsterGoal = new MonsterGoal(this);
            MonsterGoal.SetLoadState(memento.State.MonsterGoalSaveData);
        }

        public IMemento<MonsterSaveData> GetSaveState()
        {
            var memento = new Memento<MonsterSaveData>(new MonsterSaveData());

            base.PopulateSaveState(memento.State);

            memento.State.BreedName = Breed.Name;
            memento.State.MonsterGoalSaveData = MonsterGoal.GetSaveState();

            return memento;
        }

        public override void AfterMapLoaded()
        {
            base.AfterMapLoaded();

            MonsterGoal.AfterMapLoaded();
        }
    }
}