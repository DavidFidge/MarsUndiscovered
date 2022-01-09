using System;
using System.Collections.Generic;
using System.Text;

using AutoMapper;

using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using MarsUndiscovered.Components;
using MarsUndiscovered.Messages;

namespace MarsUndiscovered.Commands
{
    public class DeathCommand : BaseMarsGameActionCommand<DeathCommandSaveData>
    {
        public string KilledByMessage { get; private set; }
        public Actor Source { get; private set; }

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
            var message = $"{Source.NameAsAttackerSpecificArticle} {Source.ToHaveConjugation} died!";
            Source.IsDead = true;

            if (!(Source is Player))
            {
                GameWorld.Map.RemoveEntity(Source);
                Mediator.Publish(new MapTileChangedNotification(Source.Position));
            }

            return Result(CommandResult.Success(this, message));
        }

        protected override void UndoInternal()
        {
            Source.IsDead = false;
            GameWorld.Map.AddEntity(Source);
        }
    }
}
