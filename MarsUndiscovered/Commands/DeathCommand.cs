using AutoMapper;

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

        public override IMemento<DeathCommandSaveData> GetSaveState(IMapper mapper)
        {
            return Memento<DeathCommandSaveData>.CreateWithAutoMapper(this, mapper);
        }

        public override void SetLoadState(IMemento<DeathCommandSaveData> memento, IMapper mapper)
        {
            base.SetLoadState(memento, mapper);

            Memento<DeathCommandSaveData>.SetWithAutoMapper(this, memento, mapper);

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
