using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Assimp;

using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Graphics;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Messages;
using FrigidRogue.MonoGame.Core.UserInterface;

using MarsUndiscovered.Commands;
using MarsUndiscovered.Components;
using MarsUndiscovered.Interfaces;
using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Animation;
using MarsUndiscovered.UserInterface.Data;

using MediatR;

using SadRogue.Primitives;

using Point = SadRogue.Primitives.Point;
using Ray = Microsoft.Xna.Framework.Ray;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public abstract class BaseGameViewModel<T> : BaseViewModel<T>,
        INotificationHandler<MapTileChangedNotification>,
        INotificationHandler<EntityTransformChangedNotification>,
        INotificationHandler<ToggleShowGoalMapNotification>,
        INotificationHandler<ToggleShowEntireMapNotification>,
        INotificationHandler<GoalMapChangedNotification>,
        INotificationHandler<RefreshViewNotification>,
        INotificationHandler<FieldOfViewChangedNotifcation>,
        INotificationHandler<MapChangedNotification>
        where T : BaseGameData, new()
    {
        public Queue<TileAnimation> Animations { get; set; } = new Queue<TileAnimation>();
        public IGameWorldEndpoint GameWorldEndpoint { get; set; }
        public IGameTimeService GameTimeService { get; set; }
        public ISceneGraph SceneGraph => MapViewModel.SceneGraph;
        public MapViewModel MapViewModel { get; set; }

        public PlayerStatus PlayerStatus { get; set; }
        public IList<MonsterStatus> MonsterStatusInView { get; set; }

        public IList<string> Messages { get; set; }

        public bool IsActive { get; set; }

        protected int MessageLogCount;

        protected void SetUpViewModels()
        {
            MapViewModel.SetupNewMap(GameWorldEndpoint);
            MessageLogCount = 0;
        }

        public void QueueAnimations(IList<CommandResult> commandResults)
        {
            foreach (var commandResult in commandResults)
            {
                if (commandResult.Command is LightningAttackCommand)
                {
                    var lightningAttackCommand = commandResult.Command as LightningAttackCommand;

                    var lightningAttackAnimation = new LightningAnimation(new Lightning(lightningAttackCommand.Path));
                    Animations.Enqueue(lightningAttackAnimation);
                }
            }
        }

        public void UpdateAnimation()
        {
            if (!IsAnimating)
                return;

            var animation = Animations.Peek();

            animation.Update(GameTimeService, MapViewModel);

            if (animation.IsComplete)
            {
                Animations.Dequeue();
                animation.Finish(MapViewModel);
            }
        }

        public bool IsAnimating => Animations.Any();

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
            MonsterStatusInView = GameWorldEndpoint.GetStatusOfMonstersInView();
            PlayerStatus = GameWorldEndpoint.GetPlayerStatus();

            Messages = GameWorldEndpoint.GetMessagesSince(MessageLogCount);
            MessageLogCount += Messages.Count;

            Notify();
        }

        public string GetGameObjectInformationAt(Ray pointerRay)
        {
            var point = MapViewModel.MousePointerRayToMapPosition(pointerRay);

            if (point == null)
                return null;

            var tooltip = GameWorldEndpoint.GetGameObjectInformationAt(point.Value);

            return tooltip;
        }

        public Direction GetMapQuadrantOfRay(Ray pointerRay)
        {
            var point = MapViewModel.MousePointerRayToMapPosition(pointerRay);

            if (point == null)
                return Direction.None;

            var mapDimensions = GameWorldEndpoint.GetCurrentMapDimensions();

            var centrePoint = new Point(mapDimensions.Width / 2, mapDimensions.Height / 2);

            return Direction.GetDirection(centrePoint, point.Value);
        }

        public Task Handle(RefreshViewNotification notification, CancellationToken cancellationToken)
        {
            if (IsActive)
            {
                RefreshView();
            }

            return Unit.Task;
        }

        public Task Handle(FieldOfViewChangedNotifcation notification, CancellationToken cancellationToken)
        {
            if (IsActive)
                MapViewModel.UpdateFieldOfView(notification.NewlyVisibleTiles, notification.NewlyHiddenTiles, notification.SeenTiles);

            return Unit.Task;
        }

        public Task Handle(MapChangedNotification notification, CancellationToken cancellationToken)
        {
            if (IsActive)
                MapViewModel.UpdateAllTiles();

            return Unit.Task;
        }

        public Task Handle(ToggleShowEntireMapNotification notification, CancellationToken cancellationToken)
        {
            if (IsActive)
                MapViewModel.ToggleFieldOfView();

            return Unit.Task;
        }
    }
}