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
        private readonly IMapTileEntityFactory _mapTileEntityFactory;
        private readonly IFieldOfViewTileEntityFactory _fieldOfViewTileEntityFactory;
        private readonly IFactory<MapTileRootEntity> _mapTileRootEntityFactory;
        private readonly IFactory<MapEntity> _mapEntityFactory;
        private readonly IFactory<GoalMapEntity> _goalMapEntityFactory;
        private MapEntity _mapEntity;
        private bool _showGoalMap;
        private bool _showEntireMap;
        private ArrayView<MapTileRootEntity> _mapTileRootEntities;
        private ArrayView<MapTileEntity> _terrainTiles;
        private ArrayView<MapTileEntity> _actorTiles;
        private ArrayView<MapTileEntity> _itemTiles;
        private ArrayView<MapTileEntity> _indestructibleTiles;
        private ArrayView<FieldOfViewTileEntity> _fieldOfViewTiles;
        private ArrayView<GoalMapEntity> _goalMapTiles;
        private ArrayView<MapTileEntity> _mouseHoverTiles;
        private Path _mouseHoverPath;
        private IGameWorldEndpoint _gameWorldEndpoint;

        public MapViewModel(
            ISceneGraph sceneGraph,
            IMapTileEntityFactory mapTileEntityFactory,
            IFieldOfViewTileEntityFactory fieldOfViewTileEntityFactory,
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
            _fieldOfViewTileEntityFactory = fieldOfViewTileEntityFactory;
        }

        public void SetupNewMap(IGameWorldEndpoint gameWorldEndpoint)
        {
            _gameWorldEndpoint = gameWorldEndpoint;

            var currentMapDimensions = _gameWorldEndpoint.GetCurrentMapDimensions();

            _terrainTiles = new ArrayView<MapTileEntity>(currentMapDimensions.Width, currentMapDimensions.Height);
            _actorTiles = new ArrayView<MapTileEntity>(currentMapDimensions.Width, currentMapDimensions.Height);
            _itemTiles = new ArrayView<MapTileEntity>(currentMapDimensions.Width, currentMapDimensions.Height);
            _indestructibleTiles = new ArrayView<MapTileEntity>(currentMapDimensions.Width, currentMapDimensions.Height);
            _fieldOfViewTiles = new ArrayView<FieldOfViewTileEntity>(currentMapDimensions.Width, currentMapDimensions.Height);
            _mouseHoverTiles = new ArrayView<MapTileEntity>(currentMapDimensions.Width, currentMapDimensions.Height);
            _goalMapTiles = new ArrayView<GoalMapEntity>(currentMapDimensions.Width, currentMapDimensions.Height);

            _mapEntity = _mapEntityFactory.Create();

            _mapEntity.CreateTranslation(
                currentMapDimensions.Width,
                currentMapDimensions.Height,
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
            _indestructibleTiles[point].IsVisible = false;

            IList<IGameObject> gameObjects;

            if (_fieldOfViewTiles[point].IsVisible && _fieldOfViewTiles[point].HasBeenSeen)
            {
                gameObjects = _gameWorldEndpoint.GetLastSeenGameObjectsAtPosition(point);
            }
            else
            {
                gameObjects = _gameWorldEndpoint.GetObjectsAt(point);
            }

            UpdateTileGameObjects(point, gameObjects);
        }

        public void ToggleFieldOfView()
        {
            if (_mapEntity == null)
                return;

            _showEntireMap = !_showEntireMap;

            if (_showEntireMap)
            {
                foreach (var tile in _fieldOfViewTiles.ToArray())
                    tile.IsVisible = false;
            }
            else
            {
                _gameWorldEndpoint.UpdateFieldOfView(false);
            }

            UpdateAllTiles();
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
            _mapTileRootEntities = new ArrayView<MapTileRootEntity>(MarsMap.MapWidth, MarsMap.MapHeight);

            for (var x = 0; x < MarsMap.MapWidth; x++)
            {
                for (var y = 0; y < MarsMap.MapHeight; y++)
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

        private void CreateTileGraph(Point position, MapTileRootEntity mapTileRootEntity)
        {
            var terrainTileEntity = _mapTileEntityFactory.Create(position);
            _terrainTiles[position] = terrainTileEntity;

            var actorTileEntity = _mapTileEntityFactory.Create(position);
            _actorTiles[position] = actorTileEntity;

            var itemTileEntity = _mapTileEntityFactory.Create(position);
            _itemTiles[position] = itemTileEntity;

            var indestructibleTile = _mapTileEntityFactory.Create(position);
            _indestructibleTiles[position] = indestructibleTile;

            var fieldOfViewTileEntity = _fieldOfViewTileEntityFactory.Create(position);
            _fieldOfViewTiles[position] = fieldOfViewTileEntity;

            fieldOfViewTileEntity.SetFieldOfViewUnrevealed();

            if (_showEntireMap)
                fieldOfViewTileEntity.IsVisible = false;

            var mouseHoverEntity = _mapTileEntityFactory.Create(position);
            mouseHoverEntity.SetMouseHover();
            _mouseHoverTiles[position] = mouseHoverEntity;

            var goalMapTileEntity = _goalMapEntityFactory.Create();
            goalMapTileEntity.Initialize(position);
            _goalMapTiles[position] = goalMapTileEntity;

            _sceneGraph.Add(terrainTileEntity, mapTileRootEntity);
            _sceneGraph.Add(itemTileEntity, mapTileRootEntity);
            _sceneGraph.Add(actorTileEntity, mapTileRootEntity);
            _sceneGraph.Add(indestructibleTile, mapTileRootEntity);
            _sceneGraph.Add(mouseHoverEntity, mapTileRootEntity);

            // Field of view needs to come last to ensure it will block any tiles
            _sceneGraph.Add(fieldOfViewTileEntity, mapTileRootEntity);

            // Debug tiles
            _sceneGraph.Add(goalMapTileEntity, mapTileRootEntity);
        }

        public void UpdateAllTiles()
        {
            for (var x = 0; x < MarsMap.MapWidth; x++)
            {
                for (var y = 0; y < MarsMap.MapHeight; y++)
                {
                    UpdateTile(new Point(x, y));
                }
            }
        }

        private void UpdateTileGameObjects(Point point, IList<IGameObject> gameObjects)
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

            var indestructibleTile = gameObjects.FirstOrDefault(go => go is Indestructible);

            if (indestructibleTile != null)
            {
                if (indestructibleTile is MapExit)
                    _indestructibleTiles[point].SetMapExit(((MapExit)indestructibleTile).Direction);

                if (indestructibleTile is Ship)
                    _indestructibleTiles[point].SetShip(((Ship)indestructibleTile).ShipPart);

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

        /// <summary>
        /// Debug view of a goal map. Currently not used.
        /// </summary>
        private void UpdateTileGoalMap(Point point, IList<IGameObject> gameObjects, GoalMap goalMap)
        {
            if (!_showGoalMap)
                return;

            _goalMapTiles[point].IsVisible = false;

            var floor = gameObjects.FirstOrDefault(go => go is Floor);

            if (floor == null)
                return;

            var goalMapValue = goalMap[floor.Position];

            if (goalMapValue != null)
            {
                _goalMapTiles[point].IsVisible = true;
                _goalMapTiles[point].Text = Math.Round(goalMapValue.Value, 2).ToString();
            }
        }

        public void UpdateFieldOfView(
            IEnumerable<Point> newlyVisiblePoints,
            IEnumerable<Point> newlyHiddenPoints,
            ArrayView<SeenTile> seenTiles
        )
        {
            if (_fieldOfViewTiles == null || _showEntireMap)
                return;

            // Process hidden points first, this method can then be used to 
            // reset points to all hidden, then show visible points.
            foreach (var newlyHiddenPoint in newlyHiddenPoints)
            {
                if (seenTiles[newlyHiddenPoint].HasBeenSeen)
                {
                    _fieldOfViewTiles[newlyHiddenPoint].SetFieldOfViewHasBeenSeen();
                    UpdateTile(newlyHiddenPoint);
                }
                else
                {
                    _fieldOfViewTiles[newlyHiddenPoint].SetFieldOfViewUnrevealed();
                }
            }

            foreach (var newlyVisiblePoint in newlyVisiblePoints)
            {
                // Field of view tiles are black squares that obscure the map, so
                // to make a tile visible we hide the black tile
                _fieldOfViewTiles[newlyVisiblePoint].IsVisible = false;
                UpdateTile(newlyVisiblePoint);
            }
        }
        
        public void ShowHover(Ray ray)
        {
            UpdateMouseHoverPathTileVisibility(false);

            var mapPosition = MousePointerRayToMapPosition(ray);

            if (mapPosition == null)
                return;

            _mouseHoverPath = _gameWorldEndpoint.GetPathToPlayer(mapPosition.Value);

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