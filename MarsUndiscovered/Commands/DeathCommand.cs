
using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using GoRogue.GameFramework;

using MarsUndiscovered.Components;
using MarsUndiscovered.Interfaces;
using MarsUndiscovered.Messages;

using SadRogue.Primitives;

namespace MarsUndiscovered.Commands
{
    public class DeathCommand : BaseMarsGameActionCommand<DeathCommandSaveData>
    {
        public string KilledByMessage { get; private set; }
        public Actor Source { get; private set; }
        private Point _oldPosition;
        private Map _sourceMap;

        public DeathCommand(IGameWorld gameWorld) : base(gameWorld)
        {
        }

        public void Initialise(Actor source, string killedByMessage)
        {
            Source = source;
            KilledByMessage = $"killed by {killedByMessage}";
        }

        public override IMemento<DeathCommandSaveData> GetSaveState()
        {
            var memento = new Memento<DeathCommandSaveData>();
            base.PopulateSaveState(memento.State);
            memento.State.SourceId = Source.ID;
            memento.State.KilledByMessage = KilledByMessage;
            return memento;
        }

        public override void SetLoadState(IMemento<DeathCommandSaveData> memento)
        {
            base.PopulateLoadState(memento.State);
            KilledByMessage = memento.State.KilledByMessage;
            Source = (Actor)GameWorld.GameObjects[memento.State.SourceId];
        }

        protected override CommandResult ExecuteInternal()
        {
            var message = $"{Source.NameSpecificArticleUpperCase} {Source.ToHaveConjugation} died!";
            Source.IsDead = true;

            if (!(Source is Player))
            {
                _sourceMap = Source.CurrentMap;
                _sourceMap.RemoveEntity(Source);
                _oldPosition = Source.Position;
                Source.Position = Point.None;
                Mediator.Publish(new MapTileChangedNotification(_oldPosition));
            }

            return Result(CommandResult.Success(this, message));
        }

        protected override void UndoInternal()
        {
            Source.IsDead = false;
            Source.Position = _oldPosition;
            _sourceMap.AddEntity(Source);
        }
    }
}
