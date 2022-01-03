using System;
using System.Collections.Generic;
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
    public class ReplayViewModel : BaseViewModel<ReplayData>,
        INotificationHandler<MapTileChangedNotification>,
        INotificationHandler<EntityTransformChangedNotification>
    {
        public IGameWorldProvider GameWorldProvider { get; set; }
        public IGameWorld GameWorld => GameWorldProvider.GameWorld;
        public ISceneGraph SceneGraph => MapViewModel.SceneGraph;
        public MapViewModel MapViewModel { get; set; }

        private int _messageLogCount;

        public void LoadReplay(string filename)
        {
            GameWorldProvider.LoadReplay(filename);
            SetupNewReplay();
        }

        private void SetupNewReplay()
        {
            MapViewModel.SetupNewMap(GameWorld);

            _messageLogCount = 0;
        }

        public void ExecuteNextReplayCommand()
        {
            GameWorld.ExecuteNextReplayCommand();
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