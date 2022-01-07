using System;
using System.Collections.Generic;
using System.Linq;


using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Graphics;

using MarsUndiscovered.Components;
using MarsUndiscovered.Interfaces;

using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public class MapViewModel
    {
        public ISceneGraph SceneGraph => _sceneGraph;

        private readonly ISceneGraph _sceneGraph;
        private readonly IFactory<MapTileEntity> _mapTileEntityFactory;
        private readonly IFactory<MapTileRootEntity> _mapTileRootEntityFactory;
        private readonly IFactory<MapEntity> _mapEntityFactory;
        private readonly IFactory<GoalMapEntity> _goalMapEntityFactory;
        private IGameWorld _gameWorld;
        private MapEntity _mapEntity;
        private bool _showGoalMap;
        private ArrayView<MapTileRootEntity> _mapTileRootEntities;

        public MapViewModel(
            ISceneGraph sceneGraph,
            IFactory<MapTileEntity> mapTileEntityFactory,
            IFactory<MapTileRootEntity> mapTileRootEntityFactory,
            IFactory<MapEntity> mapEntityFactory,
            IFactory<GoalMapEntity> goalMapEntityFactory
        )
        {
            _sceneGraph = sceneGraph;
            _mapTileEntityFactory = mapTileEntityFactory;
            _mapTileRootEntityFactory = mapTileRootEntityFactory;
            _mapEntityFactory = mapEntityFactory;
            _goalMapEntityFactory = goalMapEntityFactory;
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

            CreateSubGraphForMap();
        }

        private void CreateSubGraphForMap()
        {
            _mapTileRootEntities = new ArrayView<MapTileRootEntity>(_gameWorld.Map.Width, _gameWorld.Map.Height);

            for (var x = 0; x < _gameWorld.Map.Width; x++)
            {
                for (var y = 0; y < _gameWorld.Map.Height; y++)
                {
                    var mapTileRootEntity = _mapTileRootEntityFactory.Create();
                    mapTileRootEntity.Point = new Point(x, y);
                    _mapTileRootEntities[mapTileRootEntity.Point] = mapTileRootEntity;

                    _sceneGraph.Add(mapTileRootEntity, _mapEntity);

                    CreateSubGraphForTile(new Point(x, y), mapTileRootEntity);
                }
            }
        }

        private void CreateSubGraphForTile(Point point, MapTileRootEntity mapTileRootEntity)
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

                // Goal map values are only relevant for floors
                if (_showGoalMap && gameObject is Floor)
                {
                    var goalMapValue = _gameWorld.GoalMaps.GoalMap[gameObject.Position];

                    if (goalMapValue != null)
                    {
                        var goalMapEntity = _goalMapEntityFactory.Create();
                        goalMapEntity.Initialize(gameObject, Math.Round(goalMapValue.Value, 2).ToString());
                        _sceneGraph.Add(goalMapEntity, mapTileRootEntity);
                    }
                }
            }
        }

        private void UpdateAllMapTiles()
        {
            _sceneGraph.ClearChildren(_mapEntity);

            CreateSubGraphForMap();
        }

        public void UpdateMapTiles(Point point)
        {
            if (_mapEntity == null)
                return;

            var mapTileRootEntity = _mapTileRootEntities[point];

            _sceneGraph.ClearChildren(mapTileRootEntity);

            CreateSubGraphForTile(point, mapTileRootEntity);
        }

        public void ToggleShowGoalMap()
        {
            if (_mapEntity == null)
                return;

            _showGoalMap = !_showGoalMap;

            UpdateAllMapTiles();
        }

        public void UpdateGoalMapText()
        {
            if (_mapEntity == null)
                return;

            if (!_showGoalMap)
                return;

            UpdateAllMapTiles();
        }
    }
}