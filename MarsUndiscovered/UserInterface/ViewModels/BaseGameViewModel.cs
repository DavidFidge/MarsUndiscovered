using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using FrigidRogue.MonoGame.Core.Interfaces.Graphics;
using FrigidRogue.MonoGame.Core.Messages;
using FrigidRogue.MonoGame.Core.UserInterface;

using MarsUndiscovered.Interfaces;
using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;

using MediatR;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public abstract class BaseGameViewModel<T> : BaseViewModel<T>,
        INotificationHandler<MapTileChangedNotification>,
        INotificationHandler<EntityTransformChangedNotification> where T : BaseGameData, new()
    {
        public IGameWorldProvider GameWorldProvider { get; set; }
        public IGameWorld GameWorld => GameWorldProvider.GameWorld;
        public ISceneGraph SceneGraph => MapViewModel.SceneGraph;
        public MapViewModel MapViewModel { get; set; }

        public int PlayerMaxHealth => GameWorld.Player.MaxHealth;
        public int PlayerHealth => GameWorld.Player.Health;

        protected int _messageLogCount;

        protected void SetupNewGame()
        {
            MapViewModel.SetupNewMap(GameWorld);
            _messageLogCount = 0;
            Notify();
        }

        public Task Handle(MapTileChangedNotification notification, CancellationToken cancellationToken)
        {
            MapViewModel.UpdateMapTiles(notification.Point);

            return Unit.Task;
        }

        public IList<string> GetNewMessages()
        {
            var newMessages = GameWorld.GetMessagesSince(_messageLogCount);

            _messageLogCount += newMessages.Count;

            return newMessages;
        }

        public Task Handle(EntityTransformChangedNotification notification, CancellationToken cancellationToken)
        {
            SceneGraph.HandleEntityTransformChanged(notification);
            return Unit.Task;
        }
    }
}