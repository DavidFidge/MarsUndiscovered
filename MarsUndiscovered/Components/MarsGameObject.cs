using System;
using AutoMapper;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;
using GoRogue.Components;
using GoRogue.GameFramework;
using MarsUndiscovered.Components.SaveData;
using MediatR;

using SadRogue.Primitives;

using Serilog;

namespace MarsUndiscovered.Components
{
    public abstract class MarsGameObject<T> : GameObject, IMarsGameObject<T> where T : GameObjectSaveData
    {
        public IMediator Mediator { get; set; }
        public ILogger Logger { get; set; }

        public MarsGameObject(Point position, int layer, bool isWalkable = true, bool isTransparent = true, Func<uint> idGenerator = null, IComponentCollection customComponentCollection = null) : base(position, layer, isWalkable, isTransparent, idGenerator, customComponentCollection)
        {
        }

        public MarsGameObject(int layer, bool isWalkable = true, bool isTransparent = true, Func<uint> idGenerator = null, IComponentCollection customComponentCollection = null) : base(layer, isWalkable, isTransparent, idGenerator, customComponentCollection)
        {
        }

        public virtual IMemento<T> GetState(IMapper mapper)
        {
            return Memento<T>.CreateWithAutoMapper(this, mapper);
        }

        public virtual void SetState(IMemento<T> state, IMapper mapper)
        {
            Memento<T>.SetWithAutoMapper(this, state, mapper);
        }
    }
}