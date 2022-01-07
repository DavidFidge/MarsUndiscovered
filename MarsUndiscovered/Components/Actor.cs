using System;

using GoRogue.Components;

namespace MarsUndiscovered.Components
{
    public abstract class Actor : MarsGameObject
    {
        public static int BaseHealth = 100;
        public virtual string NameAsDefender => $"the {Name.ToLower()}";
        public virtual string NameAsAttacker => $"The {Name.ToLower()}";
        public virtual string PossessiveName => NameAsDefender.EndsWith("s") ? $"{NameAsDefender}'" : $"{NameAsDefender}'s";

        public int Health { get; set; }
        public abstract int MaxHealth { get; protected set; }

        public abstract Attack BasicAttack { get; }

        public Actor(int layer, bool isWalkable = true, bool isTransparent = true, Func<uint> idGenerator = null, IComponentCollection customComponentCollection = null) : base(layer, isWalkable, isTransparent, idGenerator, customComponentCollection)
        {
        }
    }
}