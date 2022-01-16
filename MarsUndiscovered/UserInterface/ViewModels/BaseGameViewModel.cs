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

using Microsoft.Xna.Framework;

using SadRogue.Primitives;

using Point = SadRogue.Primitives.Point;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public abstract class BaseGameViewModel<T> : BaseViewModel<T>,
        INotificationHandler<MapTileChangedNotification>,
        INotificationHandler<EntityTransformChangedNotification>,
        INotificationHandler<ToggleShowGoalMapNotification>,
        INotificationHandler<GoalMapChangedNotification>,
        INotificationHandler<RefreshViewNotification>
        where T : BaseGameData, new()
        
    {
        public IGameWorldProvider GameWorldProvider { get; set; }
        public IGameWorld GameWorld => GameWorldProvider.GameWorld;
        public ISceneGraph SceneGraph => MapViewModel.SceneGraph;
        public MapViewModel MapViewModel { get; set; }

        public PlayerStatus PlayerStatus { get; set; }
        public IList<MonsterStatus> MonsterStatusInView { get; set; }

        public IList<string> Messages { get; set; }

        public bool IsActive { get; set; }

        protected int MessageLogCount;

        protected void SetUpViewModels()
        {
            MapViewModel.SetupNewMap(GameWorld);
            MessageLogCount = 0;
        }

        public Task Handle(EntityTransformChangedNotification notification, CancellationToken cancellationToken)
        {
            if (IsActive)
                SceneGraph.HandleEntityTransformChanged(notification);

            return Unit.Task;
        }

        public Task Handle(ToggleShowGoalMapNotification notification, CancellationToken cancellationToken)
        {
            if (IsActive)
                MapViewModel.ToggleShowGoalMap();

            return Unit.Task;
        }

        public Task Handle(GoalMapChangedNotification notification, CancellationToken cancellationToken)
        {
            if (IsActive)
                MapViewModel.UpdateGoalMapText();

            return Unit.Task;
        }

        public Task Handle(MapTileChangedNotification notification, CancellationToken cancellationToken)
        {
            if (IsActive)
                MapViewModel.UpdateTile(notification.Point);

            return Unit.Task;
        }

        private void RefreshView()
        {
            MonsterStatusInView = GameWorld.GetStatusOfMonstersInView();
            PlayerStatus = GameWorld.GetPlayerStatus();

            Messages = GameWorld.GetMessagesSince(MessageLogCount);
            MessageLogCount += Messages.Count;

            Notify();
        }

        public string GetGameObjectInformationAt(Ray pointerRay)
        {
            var point = MapViewModel.MousePointerRayToMapPosition(pointerRay);

            if (point == null)
                return null;

            var tooltip = GameWorld.GetGameObjectInformationAt(point.Value);

            return tooltip;
        }

        public Direction GetMapQuadrantOfRay(Ray pointerRay)
        {
            var point = MapViewModel.MousePointerRayToMapPosition(pointerRay);

            if (point == null)
                return Direction.None;

            var centrePoint = new Point(GameWorld.Map.Width / 2, GameWorld.Map.Height / 2);

            return Direction.GetDirection(centrePoint, point.Value);
        }

        public Task Handle(RefreshViewNotification notification, CancellationToken cancellationToken)
        {
            if (IsActive)
                RefreshView();

            return Unit.Task;
        }
    }
}