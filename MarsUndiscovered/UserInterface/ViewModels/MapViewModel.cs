﻿using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Graphics;
using FrigidRogue.MonoGame.Core.Graphics.Map;
using FrigidRogue.MonoGame.Core.Interfaces.Graphics;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Messages;
using GoRogue.GameFramework;
using GoRogue.Pathing;
using MarsUndiscovered.Components;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Graphics;
using MarsUndiscovered.Interfaces;
using MarsUndiscovered.UserInterface.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using Point = SadRogue.Primitives.Point;
using Rectangle = SadRogue.Primitives.Rectangle;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    // MapViewModel is a child of Replay and Game view models and is thus registered as transient.
    // Any Notifications need to be registered on the parent and forwarded here.
    public interface IMapViewModel
    {
        ISceneGraph SceneGraph { get; }
        int Width { get; }
        int Height { get; }
        void UpdateTile(Point point);
        void UpdateAllTiles();
        void ClearAnimationAttackTiles(IEnumerable<Point> points);
        void ClearAnimationAttackTile(Point point);
        void AnimateAttackTile(Point point, Action<MapTileEntity> action);
    }

    public class MapViewModel : IMapViewModel
    {
        private readonly IAssets _assets;
        private readonly ISceneGraph _sceneGraph;
        private readonly IMapTileEntityFactory _mapTileEntityFactory;
        private readonly IFieldOfViewTileEntityFactory _fieldOfViewTileEntityFactory;
        private readonly IFactory<MapEntity> _mapEntityFactory;
        private readonly IFactory<GoalMapEntity> _goalMapEntityFactory;
        private MapEntity _mapEntity;
        private bool _showGoalMap;
        private bool _showEntireMap;
        private ArrayView<MapTileEntity> _animationAttackTiles;
        private ArrayView<MapTileEntity> _terrainTiles;
        private ArrayView<MapTileEntity> _actorTiles;
        private ArrayView<MapTileEntity> _itemTiles;
        private ArrayView<MapTileEntity> _machineTiles;
        private ArrayView<MapTileEntity> _indestructibleTiles;
        private ArrayView<MapTileEntity> _environmentalEffectTiles;
        private ArrayView<FieldOfViewTileEntity> _fieldOfViewTiles;
        private ArrayView<GoalMapEntity> _goalMapTiles;
        private ArrayView<MapTileEntity> _mouseHoverTiles;
        private Path _mouseHoverPath;
        private IGameWorldProvider _gameWorldProvider;
        private IGameWorld GameWorld => _gameWorldProvider.GameWorld;
        private IList<ISpriteBatchDrawable> _allTiles;

        private int _width;
        private int _height;

        public ISceneGraph SceneGraph => _sceneGraph;

        public int Width => _width;
        public int Height => _height;
        public Rectangle Bounds => Rectangle.WithPositionAndSize(Point.Zero, _width, _height);

        public Path MouseHoverPath => _mouseHoverPath;
        
        public MapViewModel(
            IAssets assets,
            ISceneGraph sceneGraph,
            IMapTileEntityFactory mapTileEntityFactory,
            IFieldOfViewTileEntityFactory fieldOfViewTileEntityFactory,
            IFactory<MapEntity> mapEntityFactory,
            IFactory<GoalMapEntity> goalMapEntityFactory
        )
        {
            _assets = assets;
            _sceneGraph = sceneGraph;
            _mapTileEntityFactory = mapTileEntityFactory;
            _mapEntityFactory = mapEntityFactory;
            _goalMapEntityFactory = goalMapEntityFactory;
            _fieldOfViewTileEntityFactory = fieldOfViewTileEntityFactory;
            
            _mapEntity = _mapEntityFactory.Create();

            _mapEntity.Initialize(
                UiConstants.TileQuadWidth,
                UiConstants.TileQuadHeight
            );
        }

        public IList<ISpriteBatchDrawable> GetVisibleDrawableTiles()
        {
            return _allTiles;
        }

        public void SetupNewMap(
            IGameWorldProvider gameWorldProvider,
            IGameOptionsStore gameOptionsStore
            )
        {
            _assets.SetTileGraphicOptions(new TileGraphicOptions(gameOptionsStore.GetFromStore<GameOptionsData>().State));

            _mouseHoverPath = null;
            _gameWorldProvider = gameWorldProvider;

            var currentMapDimensions = GameWorld.GetCurrentMapDimensions();
            _width = currentMapDimensions.Width;
            _height = currentMapDimensions.Height;

            _animationAttackTiles = new ArrayView<MapTileEntity>(_width, _height);
            _terrainTiles = new ArrayView<MapTileEntity>(_width, _height);
            _actorTiles = new ArrayView<MapTileEntity>(_width, _height);
            _itemTiles = new ArrayView<MapTileEntity>(_width, _height);
            _machineTiles = new ArrayView<MapTileEntity>(_width, _height);
            _environmentalEffectTiles = new ArrayView<MapTileEntity>(_width, _height);
            _indestructibleTiles = new ArrayView<MapTileEntity>(_width, _height);
            _fieldOfViewTiles = new ArrayView<FieldOfViewTileEntity>(_width, _height);
            _mouseHoverTiles = new ArrayView<MapTileEntity>(_width, _height);
            _goalMapTiles = new ArrayView<GoalMapEntity>(_width, _height);
            
            _allTiles = new List<ISpriteBatchDrawable>(_width * _height * 10);

            _mapEntity.LoadContent(_width, _height);

            _sceneGraph.Initialise(_mapEntity);

            _sceneGraph.LoadContent();

            CreateMapGraph();
        }

        public void SetMapEntityTexture(Texture2D texture)
        {
            _mapEntity.SetMapTexture(texture);
        }

        public void UpdateTile(Point point)
        {
            if (_mapEntity == null)
                return;

            _animationAttackTiles[point].IsVisible = false;
            _terrainTiles[point].IsVisible = false;
            _actorTiles[point].IsVisible = false;
            _itemTiles[point].IsVisible = false;
            _machineTiles[point].IsVisible = false;
            _goalMapTiles[point].IsVisible = false;
            _indestructibleTiles[point].IsVisible = false;
            _environmentalEffectTiles[point].IsVisible = false;

            IList<IGameObject> gameObjects;

            if (_fieldOfViewTiles[point].IsVisible && _fieldOfViewTiles[point].HasBeenSeen)
            {
                gameObjects = GameWorld.GetLastSeenGameObjectsAtPosition(point);
            }
            else
            {
                gameObjects = GameWorld.GetObjectsAt(point);
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
                GameWorld.UpdateFieldOfView(false);
            }

            UpdateAllTiles();
        }

        public void ToggleShowGoalMap()
        {
            if (_mapEntity == null)
                return;

            _showGoalMap = !_showGoalMap;

            UpdateAllTiles();
            DebugUpdateTileGoalMap();
        }

        private void CreateMapGraph()
        {
            for (var x = 0; x < _width; x++)
            {
                for (var y = 0; y < _height; y++)
                {
                    var point = new Point(x, y);
                    CreateTiles(point);
                    UpdateTile(point);
                }
            }
        }

        private void CreateTiles(Point position)
        {
            var animationAttackTileEntity = _mapTileEntityFactory.Create(position);
            _animationAttackTiles[position] = animationAttackTileEntity;

            var terrainTileEntity = _mapTileEntityFactory.Create(position);
            _terrainTiles[position] = terrainTileEntity;

            var actorTileEntity = _mapTileEntityFactory.Create(position);
            _actorTiles[position] = actorTileEntity;

            var itemTileEntity = _mapTileEntityFactory.Create(position);
            _itemTiles[position] = itemTileEntity;
            
            var machineTileEntity = _mapTileEntityFactory.Create(position);
            _machineTiles[position] = machineTileEntity;

            var environmentalEffectTileEntity = _mapTileEntityFactory.Create(position);
            _environmentalEffectTiles[position] = environmentalEffectTileEntity;

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

            // The order of this list is important, it determines the draw order
            _allTiles.Add(terrainTileEntity);
            _allTiles.Add(itemTileEntity);
            _allTiles.Add(actorTileEntity);
            _allTiles.Add(indestructibleTile);
            _allTiles.Add(machineTileEntity);
            _allTiles.Add(environmentalEffectTileEntity);
            _allTiles.Add(mouseHoverEntity);
            _allTiles.Add(animationAttackTileEntity);
            _allTiles.Add(fieldOfViewTileEntity);
            
            // Debug tiles
            _allTiles.Add(goalMapTileEntity);
        }

        public void UpdateAllTiles()
        {
            for (var x = 0; x < _width; x++)
            {
                for (var y = 0; y < _height; y++)
                {
                    var point = new Point(x, y);

                    UpdateTile(point);
                }
            }
        }

        public void ClearAnimationAttackTiles(IEnumerable<Point> points)
        {
            foreach (var point in points)
            {
                ClearAnimationAttackTile(point);
            }
        }

        public void ClearAnimationAttackTile(Point point)
        {
            _animationAttackTiles[point].IsVisible = false;
        }

        public void AnimateAttackTile(Point point, Action<MapTileEntity> action)
        {
            var animationAttackTile = _animationAttackTiles[point];

            animationAttackTile.IsVisible = true;

            action(animationAttackTile);
        }

        private void UpdateTileGameObjects(Point point, IList<IGameObject> gameObjects)
        {
            var environmentalEffect = gameObjects.FirstOrDefault(go => go is EnvironmentalEffect) as EnvironmentalEffect;

            if (environmentalEffect != null)
                _environmentalEffectTiles[point].SetEnvironmentalEffect(environmentalEffect.EnvironmentalEffectType);

            var actor = gameObjects.FirstOrDefault(go => go is Actor);

            if (actor != null)
            {
                if (actor is Player player)
                    _actorTiles[point].SetPlayer(player);
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
            
            // Must come before indestructible as machines are currently indestructible
            var machine = gameObjects.FirstOrDefault(go => go is Machine);

            if (machine != null)
            {
                _machineTiles[point].SetMachine(((Machine)machine).MachineType);
                return;
            }

            var indestructibleTile = gameObjects.FirstOrDefault(go => go is Indestructible);

            if (indestructibleTile != null)
            {
                if (indestructibleTile is MapExit)
                    _indestructibleTiles[point].SetMapExit(((MapExit)indestructibleTile).MapExitType);

                else if (indestructibleTile is Ship)
                    _indestructibleTiles[point].SetShip(((Ship)indestructibleTile).AsciiCharacter);
                else
                    throw new Exception($"Indestructible tile type {0} is not being drawn");

                return;
            }
            
            // Below 4 share _terrainTiles
            var door = gameObjects.FirstOrDefault(go => go is Door) as Door;

            if (door != null && !door.IsOpen)
            {
                _terrainTiles[point].SetDoor(door.DoorType);
                return;
            }

            var feature = gameObjects.FirstOrDefault(go => go is Feature) as Feature;

            if (feature != null)
            {
                _terrainTiles[point].SetFeature(feature.FeatureType);
                return;
            }
            
            var floor = gameObjects.FirstOrDefault(go => go is Floor);

            if (floor != null)
            {
                _terrainTiles[point].SetFloor(((Floor)floor).FloorType);
                return;
            }

            var wall = gameObjects.FirstOrDefault(go => go is Wall);

            if (wall != null)
                _terrainTiles[point].SetWall(((Wall)wall).WallType);
        }

        private void DebugUpdateTileGoalMap()
        {
            if (!_showGoalMap)
                return;

            var goalMap = GameWorld.GetGoalMap();

            if (goalMap == null)
                return;

            for (var x = 0; x < goalMap.Width; x++)
            {
                for (var y = 0; y < goalMap.Height; y++)
                {
                    var point = new Point(x, y);
                    
                    _goalMapTiles[point].IsVisible = false;

                    var goalMapValue = goalMap[point];

                    if (goalMapValue != null)
                    {
                        _goalMapTiles[point].IsVisible = true;
                        _goalMapTiles[point].Text = Math.Round(goalMapValue.Value, 2).ToString();
                    }
                }
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

        public void ClearHover()
        {
            UpdateMouseHoverPathTileVisibility(false);

            _mouseHoverPath = null;
        }

        public void ShowHover(Ray ray)
        {
            UpdateMouseHoverPathTileVisibility(false);

            var mapPosition = MousePointerRayToMapPosition(ray);

            if (mapPosition == null)
            {
                _mouseHoverPath = null;
                return;
            }

            _mouseHoverPath = GameWorld.GetPathToPlayer(mapPosition.Value);

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

            var mapOffsetFromCentre = new Vector3(_mapEntity.HalfMapWidth, _mapEntity.HalfMapHeight, 0);

            var plane = new Plane(
                new Vector3(-mapOffsetFromCentre.X, -mapOffsetFromCentre.Y, 0),
                new Vector3(mapOffsetFromCentre.X, -mapOffsetFromCentre.Y, 0),
                new Vector3(-mapOffsetFromCentre.X, mapOffsetFromCentre.Y, 0)
            );

            plane = Plane.Transform(plane, worldTransform);

            var factor = ray.Intersects(plane);

            if (factor == null)
                return null;

            var intersectionPoint = ray.Position + factor.Value * ray.Direction;
            intersectionPoint = Vector3.Transform(intersectionPoint, Matrix.Invert(worldTransform));

            var untranslatedMapCoords = intersectionPoint + mapOffsetFromCentre;

            // Game world map's 0,0 is the top left coordinate but 3D map's 0,0 is the bottom left vertex.
            // So the Y calculation needs to be flipped.
            untranslatedMapCoords.Y = -(untranslatedMapCoords.Y - _mapEntity.MapHeight);

            var mapPosition = new Point(
                (int)(untranslatedMapCoords.X / UiConstants.TileQuadWidth),
                (int)(untranslatedMapCoords.Y / UiConstants.TileQuadHeight)
            );

            return mapPosition;
        }

        public void HandleEntityTransformChanged(EntityTransformChangedNotification notification)
        {
            _sceneGraph.HandleEntityTransformChanged(notification);
        }

        public void RecentreMap()
        {
            _mapEntity.SetCentreTranslation(GameWorld.GetPlayerPosition());
        }

        public void UpdateDebugTiles()
        {
            DebugUpdateTileGoalMap();
        }

        public void SetTileGraphicsOptions(TileGraphicOptions tileGraphicOptions)
        {
            _assets.SetTileGraphicOptions(tileGraphicOptions);
            UpdateAllTiles();
        }

        public void ShowHoverForSquareChoice(Ray ray)
        {
            // Currently this is identical to normal hover path code
            UpdateMouseHoverPathTileVisibility(false);

            var mapPosition = MousePointerRayToMapPosition(ray);

            if (mapPosition == null)
            {
                _mouseHoverPath = null;
                return;
            }

            _mouseHoverPath = GameWorld.GetPathForRangedAttack(mapPosition.Value);

            UpdateMouseHoverPathTileVisibility(true); 
        }
        
        public void ShowHoverForSquareChoice(Point point)
        {
            UpdateMouseHoverPathTileVisibility(false);

            _mouseHoverPath = GameWorld.GetPathForRangedAttack(point);

            UpdateMouseHoverPathTileVisibility(true);
        }

        public void MoveHover(Direction requestDirection)
        {
            UpdateMouseHoverPathTileVisibility(false);
            var playerPosition = GameWorld.GetPlayerPosition();
            var currentSelection = Point.None;

            if (_mouseHoverPath != null)
                currentSelection = _mouseHoverPath.End;
            
            if (currentSelection == Point.None)
                currentSelection = playerPosition + requestDirection;
            else
                currentSelection += requestDirection;

            // If player tries to move outside map the current selection will remain
            // unless the point is on the player themselves
            if (Bounds.Contains(currentSelection))
                _mouseHoverPath = GameWorld.GetPathForRangedAttack(currentSelection);
            
            if (playerPosition == currentSelection)
                _mouseHoverPath = null;

            UpdateMouseHoverPathTileVisibility(true);
        }
    }
}
