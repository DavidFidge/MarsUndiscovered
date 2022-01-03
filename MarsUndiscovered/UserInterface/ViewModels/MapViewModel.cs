using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Graphics;

using MarsUndiscovered.Components;
using MarsUndiscovered.Interfaces;
using MarsUndiscovered.Messages;

using MediatR;

using SadRogue.Primitives;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public class MapViewModel
    {
        public ISceneGraph SceneGraph => _sceneGraph;

        private readonly ISceneGraph _sceneGraph;
        private readonly IFactory<MapTileEntity> _mapTileEntityFactory;
        private readonly IFactory<MapTileRootEntity> _mapTileRootEntityFactory;
        private readonly IFactory<MapEntity> _mapEntityFactory;
        private IGameWorld _gameWorld;
        private MapEntity _mapEntity;

        public MapViewModel(
            ISceneGraph sceneGraph,
            IFactory<MapTileEntity> mapTileEntityFactory,
            IFactory<MapTileRootEntity> mapTileRootEntityFactory,
            IFactory<MapEntity> mapEntityFactory
            )
        {
            _sceneGraph = sceneGraph;
            _mapTileEntityFactory = mapTileEntityFactory;
            _mapTileRootEntityFactory = mapTileRootEntityFactory;
            _mapEntityFactory = mapEntityFactory;
        }

        public void SetupNewMap(IGameWorld gameWorld)
        {
            _gameWorld = gameWorld;

            _mapEntity = _mapEntityFactory.Create();

            _mapEntity.CreateTranslation(
                _gameWorld.Map.Width,
                _gameWorld.Map.Height,
                Graphics.Assets.TileQuadWidth,
                Graphics.Assets.TileQuadHeight
            );

            _sceneGraph.Initialise(_mapEntity);

            for (var x = 0; x < _gameWorld.Map.Width; x++)
            {
                for (var y = 0; y < _gameWorld.Map.Height; y++)
                {
                    var mapTileRootEntity = _mapTileRootEntityFactory.Create();
                    mapTileRootEntity.Point = new Point(x, y);

                    _sceneGraph.Add(mapTileRootEntity, _mapEntity);

                    CreateMapTiles(new Point(x, y), mapTileRootEntity);
                }
            }
        }

        private void CreateMapTiles(Point point, MapTileRootEntity mapTileRootEntity)
        {
            var gameObjects = _gameWorld.Map
                .GetObjectsAt(point)
                .Reverse()
                .ToList();

            var containsActor = gameObjects.Any(go => go is Actor);

            foreach (var gameObject in gameObjects)
            {
                var mapTileEntity = _mapTileEntityFactory.Create();

                mapTileEntity.Initialize(gameObject);

                _sceneGraph.Add(mapTileEntity, mapTileRootEntity);

                if (containsActor && gameObject is Floor)
                    mapTileEntity.IsVisible = false;
            }
        }

        public void UpdateMapTiles(Point point)
        {
            var mapTileRootEntity = (MapTileRootEntity)_sceneGraph.Find(e => e is MapTileRootEntity entity && entity.Point.Equals(point));

            if (mapTileRootEntity == null)
                return;

            _sceneGraph.ClearChildren(mapTileRootEntity);

            CreateMapTiles(point, mapTileRootEntity);
        }

        public Task Handle(MapTileChangedNotification notification, CancellationToken cancellationToken)
        {
            UpdateMapTiles(notification.Point);

            return Unit.Task;
        }
    }
}