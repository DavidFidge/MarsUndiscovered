using System;

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

        protected void PopulateSaveState(GameObjectSaveData gameObjectSaveData)
        {
            gameObjectSaveData.Position = Position;
            gameObjectSaveData.IsWalkable = IsWalkable;
            gameObjectSaveData.IsTransparent = IsTransparent;
            gameObjectSaveData.Id = ID;
        }

        protected void PopulateLoadState(GameObjectSaveData gameObjectSaveData)
        {
            Position = gameObjectSaveData.Position;
            IsWalkable = gameObjectSaveData.IsWalkable;
            IsTransparent = gameObjectSaveData.IsTransparent;
        }

        public virtual void AfterMapLoaded()
        {
        }
    }
}