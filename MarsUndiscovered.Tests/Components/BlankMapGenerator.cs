using System;
using System.Collections.Generic;

using GoRogue.GameFramework;
using MarsUndiscovered.Components;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Components.Maps;
using MarsUndiscovered.Interfaces;

using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Tests.Components
{
    public class BlankMapGenerator : IMapGenerator
    {
        private readonly IGameObjectFactory _gameObjectFactory;
        private readonly IMapGenerator _originalMapGenerator;
        private readonly Func<IGameObject> _fillWith;

        public BlankMapGenerator(IGameObjectFactory gameObjectFactory, IMapGenerator originalMapGenerator, Func<IGameObject> fillWith = null)
        {
            _gameObjectFactory = gameObjectFactory;
            _originalMapGenerator = originalMapGenerator;

            _fillWith = fillWith ?? (() => _gameObjectFactory.CreateFloor());
        }

        public ArrayView<IGameObject> CreateOutdoorWallsFloors(IGameObjectFactory gameObjectFactory)
        {
            var arrayView = new ArrayView<IGameObject>(MarsMap.MapWidth, MarsMap.MapHeight);

            arrayView.ApplyOverlay(_ => _fillWith());

            return arrayView;
        }

        public MarsMap CreateMap(IGameWorld gameWorld, IList<Wall> walls, IList<Floor> floors)
        {
            return _originalMapGenerator.CreateMap(gameWorld, walls, floors);
        }

        public MarsMap CreateMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, Func<IGameObjectFactory, ArrayView<IGameObject>> wallsFloorsGenerator)
        {
            return _originalMapGenerator.CreateMap(gameWorld, gameObjectFactory, CreateOutdoorWallsFloors);
        }
    }
}