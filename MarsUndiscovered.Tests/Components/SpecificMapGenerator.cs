using System;
using System.Collections.Generic;

using GoRogue.GameFramework;
using MarsUndiscovered.Components;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Components.Maps;
using MarsUndiscovered.Interfaces;

using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Tests.Components
{
    public class SpecificMapGenerator : IMapGenerator
    {
        public IMapGenerator OriginalMapGenerator { get; private set; }

        private readonly IGameObjectFactory _gameObjectFactory;
        private readonly Func<Point, IGameObject> _terrainChooser;

        public SpecificMapGenerator(IGameObjectFactory gameObjectFactory, IMapGenerator originalMapGenerator, IList<Point> wallPoints)
        {
            _gameObjectFactory = gameObjectFactory;
            OriginalMapGenerator = originalMapGenerator;

            _terrainChooser = ((p) => wallPoints.Contains(p) ? (Terrain)_gameObjectFactory.CreateWall() : _gameObjectFactory.CreateFloor());
        }

        public ArrayView<IGameObject> CreateOutdoorWallsFloors(IGameObjectFactory gameObjectFactory)
        {
            var arrayView = new ArrayView<IGameObject>(MarsMap.MapWidth, MarsMap.MapHeight);

            arrayView.ApplyOverlay(_terrainChooser);

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