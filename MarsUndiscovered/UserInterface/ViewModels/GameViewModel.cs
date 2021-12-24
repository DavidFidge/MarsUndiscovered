using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Graphics;
using FrigidRogue.MonoGame.Core.UserInterface;

using MarsUndiscovered.Components;
using MarsUndiscovered.Interfaces;
using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;

using MediatR;

using SadRogue.Primitives;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public class GameViewModel : BaseViewModel<GameData>, IRequestHandler<MapTileChangedRequest>
    {
        private MapEntity _mapEntity;
        public IFactory<IGameWorld> GameWorldFactory { get; set; }
        public ISceneGraph SceneGraph { get; set; }
        public IAssets Assets { get; set; }
        public IFactory<MapTileEntity> MapTileEntityFactory { get; set; }
        public IFactory<MapTileRootEntity> MapTileRootEntityFactory { get; set; }
        public IFactory<MapEntity> MapEntityFactory { get; set; }

        private IGameWorld _gameWorld;

        private int _messageLogCount;

        public void StartGame(uint? seed = null)
        {
            _gameWorld = GameWorldFactory.Create();

            _gameWorld.Generate(seed);

            _mapEntity = MapEntityFactory.Create();

            _mapEntity.CreateTranslation(_gameWorld.Map.Width, _gameWorld.Map.Height, Graphics.Assets.TileQuadWidth, Graphics.Assets.TileQuadHeight);

            SceneGraph.Initialise(_mapEntity);

            for (var x = 0; x < _gameWorld.Map.Width; x++)
            {
                for (var y = 0; y < _gameWorld.Map.Height; y++)
                {
                    var mapTileRootEntity = MapTileRootEntityFactory.Create();
                    mapTileRootEntity.Point = new Point(x, y);

                    SceneGraph.Add(mapTileRootEntity, _mapEntity);

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

            var containsPlayer = gameObjects.Any(go => go is Player);

            foreach (var gameObject in gameObjects)
            {
                var mapTileEntity = MapTileEntityFactory.Create();

                mapTileEntity.Initialize(gameObject);

                SceneGraph.Add(mapTileEntity, mapTileRootEntity);

                if (containsPlayer && gameObject is Floor)
                    mapTileEntity.IsVisible = false;
            }
        }

        public void UpdateMapTiles(Point point)
        {
            var mapTileRootEntity = (MapTileRootEntity)SceneGraph.Find(e => e is MapTileRootEntity entity && entity.Point.Equals(point));

            SceneGraph.ClearChildren(mapTileRootEntity);

            CreateMapTiles(point, mapTileRootEntity);
        }

        public void Move(Direction direction)
        {
            _gameWorld.MoveRequest(direction);
        }

        public Task<Unit> Handle(MapTileChangedRequest request, CancellationToken cancellationToken)
        {
            UpdateMapTiles(request.Point);

            return Unit.Task;
        }

        public IList<string> GetNewMessages()
        {
            var newMessages = _gameWorld.GetMessagesSince(_messageLogCount);

            _messageLogCount += newMessages.Count;

            return newMessages;
        }
    }
}