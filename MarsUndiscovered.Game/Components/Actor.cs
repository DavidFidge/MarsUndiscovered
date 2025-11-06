using MarsUndiscovered.Game.Components.SaveData;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components
{
    public abstract class Actor : MarsGameObject
    {
        protected virtual string GenericArticle => "A";
        protected virtual string SpecificArticle => "The";
        
        public virtual string ObjectiveName => Name;

        // If this actor is represented by a proper noun then no article should be used for text
        protected bool NameIsProperNoun { get; set; }
        public virtual string PossessiveName => Name.EndsWith("s") ? $"{Name}'" : $"{Name}'s";
        public virtual string ToHaveConjugation => "has";

        public virtual string GetSentenceName(bool midSentence, bool genericArticle)
        {
            if (NameIsProperNoun)
            {
                if (midSentence)
                    return ObjectiveName.ToLower();
                
                return Name;
            }

            if (midSentence)
                return genericArticle ? $"{GenericArticle.ToLower()} {Name.ToLower()}" : $"{SpecificArticle.ToLower()} {Name.ToLower()}";
            
            return genericArticle ? $"{GenericArticle} {Name.ToLower()}" : $"{SpecificArticle} {Name.ToLower()}";
        }
        
        public bool IsDead { get; set; }
        public string IsDeadMessage { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public decimal RegenRate { get; set; }
        public decimal ResidualRegen { get; set; }
        public int Shield { get; set; }

        public virtual Attack MeleeAttack { get; protected set; }
        public virtual LightningAttack LightningAttack { get; protected set; }
        public virtual LaserAttack LaserAttack { get; protected set; }
        public virtual Attack LineAttack { get; protected set; }
        public bool CanConcuss { get; set; }
        
        public abstract bool IsWallTurret { get; }

        public uint VisualRange { get; set; } = 20;

        public AllegianceCategory AllegianceCategory { get; set; }

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
            actorSaveData.VisualRange = VisualRange;
        }

        protected void PopulateLoadState(ActorSaveData actorSaveData)
        {
            base.PopulateLoadState(actorSaveData);
            MaxHealth = actorSaveData.MaxHealth;
            Health = actorSaveData.Health;
            Shield = actorSaveData.Shield;
            ResidualRegen = actorSaveData.ResidualRegen;
            IsDead = actorSaveData.IsDead;
            VisualRange = actorSaveData.VisualRange;
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

        public virtual void ApplyConcussion()
        {
        }
    }
}