using System;
using System.Collections.Generic;
using System.Linq;
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
        public IMapGenerator OriginalMapGenerator { get; private set; }
        private readonly IGameObjectFactory _gameObjectFactory;

        public BlankMapGenerator(IGameObjectFactory gameObjectFactory, IMapGenerator originalMapGenerator)
        {
            _gameObjectFactory = gameObjectFactory;
            OriginalMapGenerator = originalMapGenerator;
        }

        public MarsMap MarsMap { get; set; }
        public int Steps { get; set; }
        public bool IsComplete { get; set; }
        public void CreateOutdoorWallsFloorsMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int? upToStep = null)
        {
            var arrayView = new ArrayView<IGameObject>(MarsMap.MapWidth, MarsMap.MapHeight);

            arrayView.ApplyOverlay(_ => _gameObjectFactory.CreateFloor());

            var wallsFloors = arrayView.ToArray();

            MarsMap = MapGenerator.CreateMap(gameWorld, wallsFloors.OfType<Wall>().ToList(), wallsFloors.OfType<Floor>().ToList());

            Steps = 1;
            IsComplete = true;
        }
    }
}