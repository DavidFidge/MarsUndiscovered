using System;

using GoRogue.Components;
using SadRogue.Primitives;

namespace MarsUndiscovered.Components
{
    public abstract class Actor : MarsGameObject
    {
        public static int BaseHealth = 100;

        public int Health { get; set; }

        public abstract Attack BasicAttack { get; }

        public Actor(Point position, int layer, bool isWalkable = true, bool isTransparent = true, Func<uint> idGenerator = null, IComponentCollection customComponentCollection = null) : base(position, layer, isWalkable, isTransparent, idGenerator, customComponentCollection)
        {
        }

        public Actor(int layer, bool isWalkable = true, bool isTransparent = true, Func<uint> idGenerator = null, IComponentCollection customComponentCollection = null) : base(layer, isWalkable, isTransparent, idGenerator, customComponentCollection)
        {
        }
    }
}