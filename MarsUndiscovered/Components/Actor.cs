using System;
using AutoMapper;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using GoRogue.Components;
using MarsUndiscovered.Components.SaveData;
using SadRogue.Primitives;

namespace MarsUndiscovered.Components
{
    public abstract class Actor<T> : MarsGameObject<T> where T : GameObjectSaveData
    {
        public const int BaseHealth = 100;

        public Actor(Point position, int layer, bool isWalkable = true, bool isTransparent = true, Func<uint> idGenerator = null, IComponentCollection customComponentCollection = null) : base(position, layer, isWalkable, isTransparent, idGenerator, customComponentCollection)
        {
        }

        public Actor(int layer, bool isWalkable = true, bool isTransparent = true, Func<uint> idGenerator = null, IComponentCollection customComponentCollection = null) : base(layer, isWalkable, isTransparent, idGenerator, customComponentCollection)
        {
        }
    }
}