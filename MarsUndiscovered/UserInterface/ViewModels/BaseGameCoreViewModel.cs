using System.Threading;
using System.Threading.Tasks;
using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Components.Mediator;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Messages;
using FrigidRogue.MonoGame.Core.UserInterface;
using GoRogue.Pathing;
using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.ViewMessages;
using MarsUndiscovered.Interfaces;
using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Animation;
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
        public IGameWorldProvider GameWorldProvider { get; set; }
        public IGameWorld GameWorld => GameWorldProvider.GameWorld;
        public IGameTimeService GameTimeService { get; set; }
        public MapViewModel MapViewModel { get; set; }
        public bool IsActive { get; set; }

        public IGameOptionsStore GameOptionsStore { get; set; }

        protected void SetUpGameCoreViewModels()
        {
            MapViewModel.SetupNewMap(GameWorldProvider, GameOptionsStore);
        }

        public void QueueAnimations(IList<CommandResult> commandResults)
        {
            HashSet<Point> explodeTileCommandPoints = null;

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
                else if (commandResult.Command is ExplodeTileCommand)
                {
                    var explodeTileCommand = commandResult.Command as ExplodeTileCommand;

                    if (explodeTileCommandPoints == null)
                        explodeTileCommandPoints = new HashSet<Point>();

                    explodeTileCommandPoints.Add(explodeTileCommand.Point);
                }
            }

            if (explodeTileCommandPoints != null)
            {
                var explosionGroupAnimation = new ExplosionGroupAnimation(explodeTileCommandPoints);

                Animations.Enqueue(explosionGroupAnimation);
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

        public void Handle(EntityTransformChangedNotification notification)
        {
            if (IsActive)
                MapViewModel.HandleEntityTransformChanged(notification);
        }

        public void Handle(ToggleShowGoalMapNotification notification)
        {
            if (IsActive)
                MapViewModel.ToggleShowGoalMap();
        }

        public void Handle(MapTileChangedNotification notification)
        {
            if (IsActive)
                MapViewModel.UpdateTile(notification.Point);
        }

        protected abstract void RefreshView();

        public string GetGameObjectTooltipAt(Ray pointerRay)
        {
            var point = MapViewModel.MousePointerRayToMapPosition(pointerRay);

            if (point == null)
                return null;

            var tooltip = GameWorldProvider.GameWorld.GetGameObjectTooltipAt(point.Value);

            return tooltip;
        }

        public Direction GetMapQuadrantOfRay(Ray pointerRay)
        {
            var point = MapViewModel.MousePointerRayToMapPosition(pointerRay);

            if (point == null)
                return Direction.None;

            var mapDimensions = GameWorldProvider.GameWorld.GetCurrentMapDimensions();

            var centrePoint = new Point(mapDimensions.Width / 2, mapDimensions.Height / 2);

            return Direction.GetDirection(centrePoint, point.Value);
        }

        public void Handle(RefreshViewNotification notification)
        {
            if (IsActive)
            {
                RefreshView();
                Notify();
            }
        }

        public void Handle(FieldOfViewChangedNotification notification)
        {
            if (IsActive)
                MapViewModel.UpdateFieldOfView(notification.NewlyVisibleTiles, notification.NewlyHiddenTiles, notification.SeenTiles);
        }

        public void Handle(MapChangedNotification notification)
        {
            if (IsActive)
            {
                MapViewModel.SetupNewMap(GameWorldProvider, GameOptionsStore);
                Notify();
            }
        }

        public void Handle(ToggleShowEntireMapNotification notification)
        {
            if (IsActive)
                MapViewModel.ToggleFieldOfView();
        }

        public void Handle(ChangeTileGraphicsOptionsNotification notification)
        {
            if (IsActive)
                MapViewModel.SetTileGraphicsOptions(notification.TileGraphicOptions);
        }
    }
}