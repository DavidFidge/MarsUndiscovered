using System;

using GoRogue.Components;

namespace MarsUndiscovered.Components
{
    public abstract class Actor : MarsGameObject
    {
        public static int BaseHealth = 100;
        public virtual string NameAsDefenderSpecificArticle => $"the {Name.ToLower()}";
        public virtual string NameAsAttackerSpecificArticle => $"The {Name.ToLower()}";
        public virtual string NameAsDefenderGenericArticle => $"{GenericArticle} {Name.ToLower()}";
        public virtual string NameAsAttackerGenericArticle => $"{GenericArticle} {Name.ToLower()}";
        public virtual string GenericArticle => "a";
        public virtual string PossessiveName => NameAsDefenderSpecificArticle.EndsWith("s") ? $"{NameAsDefenderSpecificArticle}'" : $"{NameAsDefenderSpecificArticle}'s";
        public virtual string ToHaveConjugation => "has";
        public bool IsDead { get; set; } = false;
        public int Health { get; set; }
        public int MaxHealth { get; set; }

        public abstract Attack BasicAttack { get; }

        public Actor(int layer, bool isWalkable = true, bool isTransparent = true, Func<uint> idGenerator = null, IComponentCollection customComponentCollection = null) : base(layer, isWalkable, isTransparent, idGenerator, customComponentCollection)
        {
        }
    }
}