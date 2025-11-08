using FrigidRogue.MonoGame.Core.Components;
using GoRogue.GameFramework;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.ViewMessages;
using MarsUndiscovered.Interfaces;
using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Commands
{
    public class DeathCommand : BaseMarsGameActionCommand
    {
        public string KilledByMessage { get; set; }
        public Actor Source { get; set; }
        
        public DeathCommand(IGameWorld gameWorld) : base(gameWorld)
        {
        }

        public void Initialise(Actor source, string killedByMessage)
        {
            Source = source;
            KilledByMessage = $"killed by {killedByMessage}";
        }

        protected override CommandResult ExecuteInternal()
        {
            var message = $"{Source.GetSentenceName(false, false)} {Source.ToHaveConjugation} died!";
            Source.IsDead = true;
            Source.IsDeadMessage = KilledByMessage;

            if (!(Source is Player))
            {
                var oldPosition = Source.Position;

                var map = ((MarsMap)Source.CurrentMap);

                map.RemoveEntity(Source);

                Source.Position = Point.None;

                map.ActorDied(Source);

                Mediator.Publish(new MapTileChangedNotification(oldPosition));
            }
            else
            {
                GameWorld.Morgue.GameEnded();
            }
            
            GameWorld.Morgue.LogActorDeath(Source);

            return Result(CommandResult.Success(this, message));
        }
    }
}
