using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.ViewMessages;
using MarsUndiscovered.Interfaces;

using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Commands
{
    public class SwapPositionCommand : BaseMarsGameActionCommand
    {
        public Actor Source { get; set; }
        public Actor Target { get; set; }

        public SwapPositionCommand(IGameWorld gameWorld) : base(gameWorld)
        {
        }

        public void Initialise(Actor source, Actor target)
        {
            Source = source;
            Target = target;
        }

        protected override CommandResult ExecuteInternal()
        {
            this.GameWorld.CurrentMap.RemoveEntity(Source);
            this.GameWorld.CurrentMap.RemoveEntity(Target);
            var pointSource = Source.Position;
            var pointTarget = Target.Position;

            Target.Position = pointSource;
            Source.Position = pointTarget;

            this.GameWorld.CurrentMap.AddEntity(Source);
            this.GameWorld.CurrentMap.AddEntity(Target);

            var commandResult = CommandResult.Success(this);

            Mediator.Publish(new MapTileChangedNotification(pointSource));
            Mediator.Publish(new MapTileChangedNotification(pointTarget));

            return Result(commandResult);
        }
    }
}
