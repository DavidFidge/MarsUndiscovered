using System;
using System.Collections.Generic;
using System.Linq;
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
		public MarsMap MarsMap { get; set; }
        public int Steps { get; set; }
        public bool IsComplete { get; set; }

        public SpecificMapGenerator(IGameObjectFactory gameObjectFactory, IMapGenerator originalMapGenerator, IList<Point> wallPoints)
        {
            _gameObjectFactory = gameObjectFactory;
            OriginalMapGenerator = originalMapGenerator;

            var index = 0;

            _terrainChooser = ((p) =>
            {
                var terrain = wallPoints.Contains(p) ? (Terrain)_gameObjectFactory.CreateWall() : _gameObjectFactory.CreateFloor();
                terrain.Index = index++;
                return terrain;
            });
        }

        public void CreateOutdoorWallsFloorsMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int? upToStep = null)
        {
            var arrayView = new ArrayView<IGameObject>(MarsMap.MapWidth, MarsMap.MapHeight);

            arrayView.ApplyOverlay(_terrainChooser);
            
            var wallsFloors = arrayView.ToArray();

            MarsMap = MapGenerator.CreateMap(gameWorld, wallsFloors.OfType<Wall>().ToList(), wallsFloors.OfType<Floor>().ToList());
            Steps = 1;
            IsComplete = true;
        }
    }
}