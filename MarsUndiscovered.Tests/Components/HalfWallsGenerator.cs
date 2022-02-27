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
    public class HalfWallsGenerator : IMapGenerator
    {
        public IMapGenerator OriginalMapGenerator { get; private set; }

        private readonly IGameObjectFactory _gameObjectFactory;
        private readonly Func<IGameObject> _fillWith;

        public HalfWallsGenerator(IGameObjectFactory gameObjectFactory, IMapGenerator originalMapGenerator)
        {
            _gameObjectFactory = gameObjectFactory;
            OriginalMapGenerator = originalMapGenerator;
        }

        public ArrayView<IGameObject> CreateOutdoorWallsFloors(IGameObjectFactory gameObjectFactory)
        {
            var arrayView = new ArrayView<IGameObject>(MarsMap.MapWidth, MarsMap.MapHeight);

            var index = 0;

            arrayView.ApplyOverlay(p =>
            {
                var terrain = p.Y > MarsMap.MapHeight - 5 ? (Terrain)_gameObjectFactory.CreateWall() : _gameObjectFactory.CreateFloor();
                terrain.Index = index++;
                return terrain;
            });

            return arrayView;
        }

        public MarsMap CreateMap(IGameWorld gameWorld, IList<Wall> walls, IList<Floor> floors)
        {
            return OriginalMapGenerator.CreateMap(gameWorld, walls, floors);
        }

        public MarsMap CreateMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, Func<IGameObjectFactory, ArrayView<IGameObject>> wallsFloorsGenerator)
        {
            return OriginalMapGenerator.CreateMap(gameWorld, gameObjectFactory, CreateOutdoorWallsFloors);
        }
    }
}