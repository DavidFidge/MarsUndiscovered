using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using FrigidRogue.MonoGame.Core.Interfaces.Graphics;
using FrigidRogue.MonoGame.Core.Messages;
using FrigidRogue.MonoGame.Core.UserInterface;

using MarsUndiscovered.Components;
using MarsUndiscovered.Interfaces;
using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;

using MediatR;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public abstract class BaseGameViewModel<T> : BaseViewModel<T>,
        INotificationHandler<MapTileChangedNotification>,
        INotificationHandler<EntityTransformChangedNotification>,
        INotificationHandler<ToggleShowGoalMapNotification>,
        INotificationHandler<GoalMapChangedNotification>
        where T : BaseGameData, new()
        
    {
        public IGameWorldProvider GameWorldProvider { get; set; }
        public IGameWorld GameWorld => GameWorldProvider.GameWorld;
        public ISceneGraph SceneGraph => MapViewModel.SceneGraph;
        public MapViewModel MapViewModel { get; set; }

        public PlayerStatus PlayerStatus { get; set; }
        public IList<MonsterStatus> MonsterStatusInView { get; set; }

        public IList<string> Messages { get; set; }

        protected int MessageLogCount;

        protected void SetupNewGame()
        {
            MapViewModel.SetupNewMap(GameWorld);
            MessageLogCount = 0;
            GetNewTurnData();
        }

        public Task Handle(EntityTransformChangedNotification notification, CancellationToken cancellationToken)
        {
            SceneGraph.HandleEntityTransformChanged(notification);
            return Unit.Task;
        }

        public Task Handle(ToggleShowGoalMapNotification notification, CancellationToken cancellationToken)
        {
            MapViewModel.ToggleShowGoalMap();
            return Unit.Task;
        }

        public Task Handle(GoalMapChangedNotification notification, CancellationToken cancellationToken)
        {
            MapViewModel.UpdateGoalMapText();
            return Unit.Task;
        }

        public Task Handle(MapTileChangedNotification notification, CancellationToken cancellationToken)
        {
            MapViewModel.UpdateTile(notification.Point);
            return Unit.Task;
        }

        protected void GetNewTurnData()
        {
            MonsterStatusInView = GameWorld.GetStatusOfMonstersInView();
            PlayerStatus = GameWorld.GetPlayerStatus();

            Messages = GameWorld.GetMessagesSince(MessageLogCount);
            MessageLogCount += Messages.Count;

            Notify();
        }
    }
}