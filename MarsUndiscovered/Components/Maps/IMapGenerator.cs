using System;
using System.Collections.Generic;

using GoRogue.GameFramework;

using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Interfaces;

using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Components.Maps
{
    public interface IMapGenerator
    {
        MarsMap CreateMap(IGameWorld gameWorld, IList<Wall> walls, IList<Floor> floors);
        MarsMap CreateMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, Func<IGameObjectFactory, ArrayView<IGameObject>> wallsFloorsGenerator);
    }
}