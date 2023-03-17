using MarsUndiscovered.Components.SaveData;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Components
{
    public abstract class Actor : MarsGameObject
    {
        public virtual string NameSpecificArticleLowerCase => $"the {Name.ToLower()}";
        public virtual string NameSpecificArticleUpperCase => $"The {Name.ToLower()}";
        public virtual string NameGenericArticleLowerCase => $"{GenericArticleLowerCase} {Name.ToLower()}";
        public virtual string NameGenericArticleUpperCase => $"{GenericArticleUpperCase} {Name.ToLower()}";
        protected virtual string GenericArticleLowerCase => "a";
        protected virtual string GenericArticleUpperCase => "A";
        public virtual string PossessiveName => NameSpecificArticleLowerCase.EndsWith("s") ? $"{NameSpecificArticleLowerCase}'" : $"{NameSpecificArticleLowerCase}'s";
        public virtual string ToHaveConjugation => "has";
        public bool IsDead { get; set; } = false;
        public string IsDeadMessage { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public decimal RegenRate { get; set; }
        public decimal ResidualRegen { get; set; }
        public int Shield { get; set; }

        public virtual Attack MeleeAttack { get; protected set; }
        public virtual LightningAttack LightningAttack { get; protected set; }
        public virtual Attack LineAttack { get; protected set; }

        public abstract bool IsWallTurret { get; }

        public Actor(IGameWorld gameWorld, uint id) : base(gameWorld, Constants.ActorLayer, false, true, () => id)
        {
        }

        protected void PopulateSaveState(ActorSaveData actorSaveData)
        {
            base.PopulateSaveState(actorSaveData);

            actorSaveData.MaxHealth = MaxHealth;
            actorSaveData.Health = Health;
            actorSaveData.Shield = Shield;
            actorSaveData.ResidualRegen = ResidualRegen;
            actorSaveData.IsDead = IsDead;
        }

        protected void PopulateLoadState(ActorSaveData actorSaveData)
        {
            base.PopulateLoadState(actorSaveData);
            MaxHealth = actorSaveData.MaxHealth;
            Health = actorSaveData.Health;
            Shield = actorSaveData.Shield;
            ResidualRegen = actorSaveData.ResidualRegen;
            IsDead = actorSaveData.IsDead;
        }
        
        public void Regenerate()
        {
            ResidualRegen += RegenRate * MaxHealth;
            var regenAmount = (int)ResidualRegen;
            Health += regenAmount;
            ResidualRegen -= regenAmount;

            if (Health >= MaxHealth)
            {
                Health = MaxHealth;
                ResidualRegen = 0;
            }
        }

        public void ApplyDamage(int damage)
        {
            Shield -= damage;

            if (Shield > 0)
                return;
            
            damage = Math.Abs(Shield);
            Shield = 0;
            
            Health -= damage;
        }
    }
}