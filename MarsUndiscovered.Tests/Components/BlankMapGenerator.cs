using System;

using GoRogue.GameFramework;
using MarsUndiscovered.Components;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Components.Maps;
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

        public ArrayView<IGameObject> CreateOutdoorWallsFloors()
        {
            var arrayView = new ArrayView<IGameObject>(MapGenerator.MapWidth, MapGenerator.MapHeight);

            arrayView.ApplyOverlay(_ => _fillWith());

            return arrayView;
        }

        public Map CreateMap(WallCollection walls, FloorCollection floors)
        {
            return _originalMapGenerator.CreateMap(walls, floors);
        }
    }
}