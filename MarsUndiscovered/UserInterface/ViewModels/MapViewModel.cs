using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Graphics;
using FrigidRogue.MonoGame.Core.Interfaces.Graphics;
using FrigidRogue.MonoGame.Core.Messages;
using GoRogue.GameFramework;
using GoRogue.Pathing;

using MarsUndiscovered.Components;
using MarsUndiscovered.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadRogue.Primitives.GridViews;

using Point = SadRogue.Primitives.Point;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public class MapViewModel
    {
        private readonly ISceneGraph _sceneGraph;
        private readonly IMapTileEntityFactory _mapTileEntityFactory;
        private readonly IFieldOfViewTileEntityFactory _fieldOfViewTileEntityFactory;
        private readonly IFactory<MapEntity> _mapEntityFactory;
        private readonly IFactory<GoalMapEntity> _goalMapEntityFactory;
        private MapEntity _mapEntity;
        private bool _showGoalMap;
        private bool _showEntireMap;
        private ArrayView<MapTileEntity> _animationTiles;
        private ArrayView<MapTileEntity> _terrainTiles;
        private ArrayView<MapTileEntity> _actorTiles;
        private ArrayView<MapTileEntity> _itemTiles;
        private ArrayView<MapTileEntity> _indestructibleTiles;
        private ArrayView<FieldOfViewTileEntity> _fieldOfViewTiles;
        private ArrayView<GoalMapEntity> _goalMapTiles;
        private ArrayView<MapTileEntity> _mouseHoverTiles;
        private Path _mouseHoverPath;
        private IGameWorldEndpoint _gameWorldEndpoint;
        private IList<ISpriteBatchDrawable> _allTiles;

        private int _width;
        private int _height;

        public ISceneGraph SceneGraph => _sceneGraph;

        public int Width => _width;
        public int Height => _height;

        public MapViewModel(
            ISceneGraph sceneGraph,
            IMapTileEntityFactory mapTileEntityFactory,
            IFieldOfViewTileEntityFactory fieldOfViewTileEntityFactory,
            IFactory<MapEntity> mapEntityFactory,
            IFactory<GoalMapEntity> goalMapEntityFactory
        )
        {
            _sceneGraph = sceneGraph;
            _mapTileEntityFactory = mapTileEntityFactory;
            _mapEntityFactory = mapEntityFactory;
            _goalMapEntityFactory = goalMapEntityFactory;
            _fieldOfViewTileEntityFactory = fieldOfViewTileEntityFactory;
            
            _mapEntity = _mapEntityFactory.Create();

            _mapEntity.Initialize(
                Constants.TileQuadWidth,
                Constants.TileQuadHeight
            );
        }

        public IList<ISpriteBatchDrawable> GetVisibleDrawableTiles()
        {
            return _allTiles;
        }

        public void SetupNewMap(IGameWorldEndpoint gameWorldEndpoint)
        {
            _mouseHoverPath = null;
            _gameWorldEndpoint = gameWorldEndpoint;

            var currentMapDimensions = _gameWorldEndpoint.GetCurrentMapDimensions();
            _width = currentMapDimensions.Width;
            _height = currentMapDimensions.Height;

            _animationTiles = new ArrayView<MapTileEntity>(_width, _height);
            _terrainTiles = new ArrayView<MapTileEntity>(_width, _height);
            _actorTiles = new ArrayView<MapTileEntity>(_width, _height);
            _itemTiles = new ArrayView<MapTileEntity>(_width, _height);
            _indestructibleTiles = new ArrayView<MapTileEntity>(_width, _height);
            _fieldOfViewTiles = new ArrayView<FieldOfViewTileEntity>(_width, _height);
            _mouseHoverTiles = new ArrayView<MapTileEntity>(_width, _height);
            _goalMapTiles = new ArrayView<GoalMapEntity>(_width, _height);
            
            _allTiles = new List<ISpriteBatchDrawable>(_width * _height * 8);
            
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

            _animationTiles[point].IsVisible = false;
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
            var animationTileEntity = _mapTileEntityFactory.Create(position);
            _animationTiles[position] = animationTileEntity;

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

            _allTiles.Add(terrainTileEntity);
            _allTiles.Add(itemTileEntity);
            _allTiles.Add(actorTileEntity);
            _allTiles.Add(indestructibleTile);
            _allTiles.Add(mouseHoverEntity);
            _allTiles.Add(animationTileEntity);
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
                    UpdateTile(new Point(x, y));
                }
            }
        }

        public void ClearAnimationTile(Point point)
        {
            ClearAnimationTiles(new[] { point });
        }

        public void ClearAnimationTiles(IEnumerable<Point> points)
        {
            foreach (var point in points)
            {
                _animationTiles[point].IsVisible = false;
            }
        }

        public void AnimateTile(Point point, Action<MapTileEntity> action)
        {
            var animationTile = _animationTiles[point];

            animationTile.IsVisible = true;

            action(animationTile);
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
                (int)(untranslatedMapCoords.X / Constants.TileQuadWidth),
                (int)(untranslatedMapCoords.Y / Constants.TileQuadHeight)
            );

            return mapPosition;
        }

        public void HandleEntityTransformChanged(EntityTransformChangedNotification notification)
        {
            _sceneGraph.HandleEntityTransformChanged(notification);
        }

        public void RecentreMap()
        {
            _mapEntity.SetCentreTranslation(_gameWorldEndpoint.GetPlayerPosition());
        }
    }
}
