using System;
using System.Collections.Generic;
using System.Linq;

using FrigidRogue.MonoGame.Core.Components;

using GoRogue.Components;
using GoRogue.GameFramework;

using MediatR;

using SadRogue.Primitives;

using Serilog;

namespace MarsUndiscovered.Components
{
    public class MarsGameObject : GameObject, IBaseComponent
    {
        public IMediator Mediator { get; set; }
        public ILogger Logger { get; set; }

        public MarsGameObject(Point position, int layer, bool isWalkable = true, bool isTransparent = true, Func<uint>? idGenerator = null, IComponentCollection? customComponentCollection = null) : base(position, layer, isWalkable, isTransparent, idGenerator, customComponentCollection)
        {
        }

        public MarsGameObject(int layer, bool isWalkable = true, bool isTransparent = true, Func<uint>? idGenerator = null, IComponentCollection? customComponentCollection = null) : base(layer, isWalkable, isTransparent, idGenerator, customComponentCollection)
        {
        }
    }
}