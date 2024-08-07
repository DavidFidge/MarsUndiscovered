﻿using System.Threading;
using System.Threading.Tasks;
using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Messages;
using FrigidRogue.MonoGame.Core.UserInterface;
using GoRogue.Pathing;
using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.ViewMessages;
using MarsUndiscovered.Interfaces;
using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Animation;
using MediatR;

using SadRogue.Primitives;

using Point = SadRogue.Primitives.Point;
using Ray = Microsoft.Xna.Framework.Ray;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public abstract class BaseGameCoreViewModel<T> : BaseViewModel<T>,
        INotificationHandler<MapTileChangedNotification>,
        INotificationHandler<EntityTransformChangedNotification>,
        INotificationHandler<ToggleShowGoalMapNotification>,
        INotificationHandler<ToggleShowEntireMapNotification>,
        INotificationHandler<RefreshViewNotification>,
        INotificationHandler<FieldOfViewChangedNotification>,
        INotificationHandler<MapChangedNotification>,
        INotificationHandler<ChangeTileGraphicsOptionsNotification>
        where T : new()
    {
        public Queue<TileAnimation> Animations { get; set; } = new Queue<TileAnimation>();
        public IGameWorldEndpoint GameWorldEndpoint { get; set; }
        public IGameTimeService GameTimeService { get; set; }
        public MapViewModel MapViewModel { get; set; }
        public bool IsActive { get; set; }

        public IGameOptionsStore GameOptionsStore { get; set; }

        protected void SetUpGameCoreViewModels()
        {
            MapViewModel.SetupNewMap(GameWorldEndpoint, GameOptionsStore);
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
                if (commandResult.Command is LaserAttackCommand)
                {
                    var laserAttackCommand = commandResult.Command as LaserAttackCommand;

                    var laserAttackAnimation = new LaserAnimation(new Path(laserAttackCommand.Path));
                    Animations.Enqueue(laserAttackAnimation);
                }
                else if (commandResult.Command is LineAttackCommand)
                {
                    var lineAttackCommand = commandResult.Command as LineAttackCommand;

                    var lineAttackAnimation = new LineAttackAnimation(lineAttackCommand.Path);
                    Animations.Enqueue(lineAttackAnimation);
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
                MapViewModel.HandleEntityTransformChanged(notification);
                
            return Unit.Task;
        }

        public Task Handle(ToggleShowGoalMapNotification notification, CancellationToken cancellationToken)
        {
            if (IsActive)
                MapViewModel.ToggleShowGoalMap();

            return Unit.Task;
        }

        public Task Handle(MapTileChangedNotification notification, CancellationToken cancellationToken)
        {
            if (IsActive)
                MapViewModel.UpdateTile(notification.Point);

            return Unit.Task;
        }

        protected abstract void RefreshView();

        public string GetGameObjectTooltipAt(Ray pointerRay)
        {
            var point = MapViewModel.MousePointerRayToMapPosition(pointerRay);

            if (point == null)
                return null;

            var tooltip = GameWorldEndpoint.GetGameObjectTooltipAt(point.Value);

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
                Notify();
            }

            return Unit.Task;
        }

        public Task Handle(FieldOfViewChangedNotification notification, CancellationToken cancellationToken)
        {
            if (IsActive)
                MapViewModel.UpdateFieldOfView(notification.NewlyVisibleTiles, notification.NewlyHiddenTiles, notification.SeenTiles);

            return Unit.Task;
        }

        public Task Handle(MapChangedNotification notification, CancellationToken cancellationToken)
        {
            if (IsActive)
            {
                MapViewModel.SetupNewMap(GameWorldEndpoint, GameOptionsStore);
                Notify();
            }

            return Unit.Task;
        }

        public Task Handle(ToggleShowEntireMapNotification notification, CancellationToken cancellationToken)
        {
            if (IsActive)
                MapViewModel.ToggleFieldOfView();

            return Unit.Task;
        }

        public Task Handle(ChangeTileGraphicsOptionsNotification notification, CancellationToken cancellationToken)
        {
            if (IsActive)
                MapViewModel.SetTileGraphicsOptions(notification.TileGraphicOptions);

            return Unit.Task;
        }
    }
}