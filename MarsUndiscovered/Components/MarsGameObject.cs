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
    public abstract class MarsGameObject : GameObject, IMarsGameObject
    {
        public IMediator Mediator { get; set; }
        public ILogger Logger { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }

        public MarsGameObject(Point position, int layer, bool isWalkable = true, bool isTransparent = true, Func<uint> idGenerator = null, IComponentCollection customComponentCollection = null) : base(position, layer, isWalkable, isTransparent, idGenerator, customComponentCollection)
        {
        }

        public MarsGameObject(int layer, bool isWalkable = true, bool isTransparent = true, Func<uint> idGenerator = null, IComponentCollection customComponentCollection = null) : base(layer, isWalkable, isTransparent, idGenerator, customComponentCollection)
        {
        }

        protected IMemento<T> CreateWithAutoMapper<T>(IMapper mapper) where T : GameObjectSaveData
        {
            var mementoForDerivedType = Memento<T>.CreateWithAutoMapper(this, mapper);

            return new Memento<T>(mementoForDerivedType.State);
        }

        protected void SetWithAutoMapper<T>(IMemento<T> state, IMapper mapper) where T : GameObjectSaveData
        {
            var memento = new Memento<T>(state.State);

            Memento<T>.SetWithAutoMapper(this, memento, mapper);
        }
    }
}