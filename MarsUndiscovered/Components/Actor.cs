using MarsUndiscovered.Components.SaveData;

namespace MarsUndiscovered.Components
{
    public abstract class Actor : MarsGameObject
    {
        public static int BaseHealth = 100;
        public virtual string NameSpecificArticleLowerCase => $"the {Name.ToLower()}";
        public virtual string NameSpecificArticleUpperCase => $"The {Name.ToLower()}";
        public virtual string NameGenericArticleLowerCase => $"{GenericArticleLowerCase} {Name.ToLower()}";
        public virtual string NameGenericArticleUpperCase => $"{GenericArticleUpperCase} {Name.ToLower()}";
        protected virtual string GenericArticleLowerCase => "a";
        protected virtual string GenericArticleUpperCase => "A";
        public virtual string PossessiveName => NameSpecificArticleLowerCase.EndsWith("s") ? $"{NameSpecificArticleLowerCase}'" : $"{NameSpecificArticleLowerCase}'s";
        public virtual string ToHaveConjugation => "has";
        public bool IsDead { get; set; } = false;
        public int Health { get; set; }
        public int MaxHealth { get; set; }

        public abstract Attack BasicAttack { get; }

        public Actor(uint id) : base(Constants.ActorLayer, false, true, () => id)
        {
        }

        protected void PopulateSaveState(ActorSaveData actorSaveData)
        {
            base.PopulateSaveState(actorSaveData);

            actorSaveData.MaxHealth = MaxHealth;
            actorSaveData.Health = Health;
            actorSaveData.IsDead = IsDead;
        }

        protected void PopulateLoadState(ActorSaveData actorSaveData)
        {
            base.PopulateLoadState(actorSaveData);
            MaxHealth = actorSaveData.MaxHealth;
            Health = actorSaveData.Health;
            IsDead = actorSaveData.IsDead;
        }
    }
}