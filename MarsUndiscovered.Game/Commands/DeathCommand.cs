using FrigidRogue.MonoGame.Core.Components;
using GoRogue.GameFramework;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.ViewMessages;
using MarsUndiscovered.Interfaces;
using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Commands
{
    public class DeathCommand : BaseMarsGameActionCommand<DeathCommandSaveData>
    {
        public string KilledByMessage => _data.KilledByMessage;
        public Actor Source => GameWorld.GameObjects[_data.SourceId] as Actor;
        private Point _oldPosition => _data.OldPosition;
        private Map _sourceMap => GameWorld.Maps.Single(m => m.Id == _data.MapId);
        
        public DeathCommand(IGameWorld gameWorld) : base(gameWorld)
        {
        }

        public void Initialise(Actor source, string killedByMessage)
        {
            _data.SourceId = source.ID;
            _data.KilledByMessage = $"killed by {killedByMessage}";
        }

        protected override CommandResult ExecuteInternal()
        {
            var message = $"{Source.GetSentenceName(false, false)} {Source.ToHaveConjugation} died!";
            Source.IsDead = true;
            Source.IsDeadMessage = KilledByMessage;

            if (!(Source is Player))
            {
                _data.MapId = ((MarsMap)Source.CurrentMap).Id;
                _sourceMap.RemoveEntity(Source);
                _data.OldPosition = Source.Position;
                Source.Position = Point.None;
                
                Mediator.Publish(new MapTileChangedNotification(_oldPosition));
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
