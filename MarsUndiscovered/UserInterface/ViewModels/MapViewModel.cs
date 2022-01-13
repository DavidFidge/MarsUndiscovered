using System;
using System.Collections.Generic;
using System.Linq;

using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Graphics;

using GoRogue.GameFramework;
using GoRogue.Pathing;

using MarsUndiscovered.Components;
using MarsUndiscovered.Interfaces;

using Microsoft.Xna.Framework;

using SadRogue.Primitives.GridViews;

using Point = SadRogue.Primitives.Point;

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
        private ArrayView<MapTileEntity> _terrainTiles;
        private ArrayView<MapTileEntity> _actorTiles;
        private ArrayView<MapTileEntity> _itemTiles;
        private ArrayView<GoalMapEntity> _goalMapTiles;
        private ArrayView<MapTileEntity> _mouseHoverTiles;
        private Path _mouseHoverPath;

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

            _terrainTiles = new ArrayView<MapTileEntity>(_gameWorld.Map.Width, _gameWorld.Map.Height);
            _actorTiles = new ArrayView<MapTileEntity>(_gameWorld.Map.Width, _gameWorld.Map.Height);
            _itemTiles = new ArrayView<MapTileEntity>(_gameWorld.Map.Width, _gameWorld.Map.Height);
            _mouseHoverTiles = new ArrayView<MapTileEntity>(_gameWorld.Map.Width, _gameWorld.Map.Height);
            _goalMapTiles = new ArrayView<GoalMapEntity>(_gameWorld.Map.Width, _gameWorld.Map.Height);

            _mapEntity = _mapEntityFactory.Create();

            _mapEntity.CreateTranslation(
                _gameWorld.Map.Width,
                _gameWorld.Map.Height,
                Graphics.Assets.TileQuadWidth,
                Graphics.Assets.TileQuadHeight
            );

            _sceneGraph.Initialise(_mapEntity);

            CreateMapGraph();
        }

        public void UpdateTile(Point point)
        {
            if (_mapEntity == null)
                return;

            _terrainTiles[point].IsVisible = false;
            _actorTiles[point].IsVisible = false;
            _itemTiles[point].IsVisible = false;
            _goalMapTiles[point].IsVisible = false;

            var gameObjects = _gameWorld.Map
                .GetObjectsAt(point)
                .ToList();

            UpdateTileGoalMap(point, gameObjects);
            UpdateTileGameObjects(point, gameObjects);
        }

        public void ToggleShowGoalMap()
        {
            if (_mapEntity == null)
                return;

            _showGoalMap = !_showGoalMap;

            UpdateAllTiles();
        }

        public void UpdateGoalMapText()
        {
            if (_mapEntity == null)
                return;

            if (!_showGoalMap)
                return;

            UpdateAllTiles();
        }

        private void CreateMapGraph()
        {
            _mapTileRootEntities = new ArrayView<MapTileRootEntity>(_gameWorld.Map.Width, _gameWorld.Map.Height);

            for (var x = 0; x < _gameWorld.Map.Width; x++)
            {
                for (var y = 0; y < _gameWorld.Map.Height; y++)
                {
                    var point = new Point(x, y);
                    var mapTileRootEntity = _mapTileRootEntityFactory.Create();
                    mapTileRootEntity.Point = point;
                    _mapTileRootEntities[mapTileRootEntity.Point] = mapTileRootEntity;

                    _sceneGraph.Add(mapTileRootEntity, _mapEntity);

                    CreateTileGraph(point, mapTileRootEntity);
                    UpdateTile(point);
                }
            }
        }

        private void CreateTileGraph(Point point, MapTileRootEntity mapTileRootEntity)
        {
            var terrainTileEntity = _mapTileEntityFactory.Create();
            terrainTileEntity.Initialize(point);
            _terrainTiles[point] = terrainTileEntity;

            var actorTileEntity = _mapTileEntityFactory.Create();
            actorTileEntity.Initialize(point);
            _actorTiles[point] = actorTileEntity;

            var itemTileEntity = _mapTileEntityFactory.Create();
            itemTileEntity.Initialize(point);
            _itemTiles[point] = itemTileEntity;

            var mouseHoverEntity = _mapTileEntityFactory.Create();
            mouseHoverEntity.Initialize(point);
            mouseHoverEntity.SetMouseHover();
            mouseHoverEntity.IsVisible = false;
            _mouseHoverTiles[point] = mouseHoverEntity;

            var goalMapTileEntity = _goalMapEntityFactory.Create();
            goalMapTileEntity.Initialize(point);
            _goalMapTiles[point] = goalMapTileEntity;


            _sceneGraph.Add(terrainTileEntity, mapTileRootEntity);
            _sceneGraph.Add(itemTileEntity, mapTileRootEntity);
            _sceneGraph.Add(actorTileEntity, mapTileRootEntity);
            _sceneGraph.Add(mouseHoverEntity, mapTileRootEntity);
            _sceneGraph.Add(goalMapTileEntity, mapTileRootEntity);
        }

        private void UpdateAllTiles()
        {
            for (var x = 0; x < _gameWorld.Map.Width; x++)
            {
                for (var y = 0; y < _gameWorld.Map.Height; y++)
                {
                    UpdateTile(new Point(x, y));
                }
            }
        }

        private void UpdateTileGameObjects(Point point, List<IGameObject> gameObjects)
        {
            var actor = gameObjects.FirstOrDefault(go => go is Actor);

            if (actor != null)
            {
                if (actor is Player)
                    _actorTiles[point].SetPlayer();
                else if (actor is Monster monster)
                    _actorTiles[point].SetMonster(monster.Breed);

                return;
            }

            var item = gameObjects.FirstOrDefault(go => go is Item);

            if (item != null)
            {
                _itemTiles[point].SetItem(((Item)item).ItemType);

                return;
            }

            var floor = gameObjects.FirstOrDefault(go => go is Floor);

            if (floor != null)
            {
                _terrainTiles[point].SetFloor();
                return;
            }

            var wall = gameObjects.FirstOrDefault(go => go is Wall);

            if (wall != null)
                _terrainTiles[point].SetWall();
        }

        private void UpdateTileGoalMap(Point point, List<IGameObject> gameObjects)
        {
            if (!_showGoalMap)
                return;

            _goalMapTiles[point].IsVisible = false;

            var floor = gameObjects.FirstOrDefault(go => go is Floor);

            if (floor == null)
                return;

            var goalMapValue = _gameWorld.GoalMaps.GoalMap[floor.Position];

            if (goalMapValue != null)
            {
                _goalMapTiles[point].IsVisible = true;
                _goalMapTiles[point].Text = Math.Round(goalMapValue.Value, 2).ToString();
            }
        }

        public void ShowHover(Ray ray)
        {
            UpdateMouseHoverPathTileVisibility(false);

            var mapPosition = MousePointerRayToMapPosition(ray);

            if (mapPosition == null)
                return;

            _mouseHoverPath = _gameWorld.GetPathToPlayer(mapPosition.Value);

            UpdateMouseHoverPathTileVisibility(true);
        }

        private void UpdateMouseHoverPathTileVisibility(bool isVisible)
        {
            if (_mouseHoverPath == null)
                return;

            foreach (var point in _mouseHoverPath.Steps)
            {
                _mouseHoverTiles[point].IsVisible = isVisible;
            }
        }

        public Point? MousePointerRayToMapPosition(Ray ray)
        {
            if (_mapEntity == null)
                return null;

            var worldTransform = _sceneGraph.GetWorldTransform(_mapEntity);

            var plane = new Plane(
                new Vector3(0, 0, worldTransform.Translation.Z),
                new Vector3(worldTransform.Translation.X, 0, worldTransform.Translation.Z),
                new Vector3(0, worldTransform.Translation.Y, worldTransform.Translation.Z)
            );

            var factor = ray.Intersects(plane);

            if (factor == null)
                return null;

            var intersectionPoint = ray.Position + factor.Value * ray.Direction;

            var untranslatedMapCoords = intersectionPoint - worldTransform.Translation;

            var mapPosition = new Point(
                (int)((untranslatedMapCoords.X + Graphics.Assets.TileQuadWidth / 2) / Graphics.Assets.TileQuadWidth),
                (int)((-untranslatedMapCoords.Y + Graphics.Assets.TileQuadHeight / 2) / Graphics.Assets.TileQuadHeight)
            );

            return mapPosition;
        }
    }
}