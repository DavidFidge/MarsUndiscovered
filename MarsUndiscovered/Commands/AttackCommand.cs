using System;
using System.Collections.Generic;
using System.Text;

using AutoMapper;

using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using MarsUndiscovered.Components;

namespace MarsUndiscovered.Commands
{
    public class AttackCommand : BaseMarsGameActionCommand<AttackCommandSaveData>
    {
        public Actor Source { get; private set; }
        public Actor Target { get; private set; }

        private int _damage;

        public void Initialise(Actor source, Actor target)
        {
            Source = source;
            Target = target;
        }

        public override IMemento<AttackCommandSaveData> GetSaveState(IMapper mapper)
        {
            return Memento<AttackCommandSaveData>.CreateWithAutoMapper(this, mapper);
        }

        public override void SetLoadState(IMemento<AttackCommandSaveData> memento, IMapper mapper)
        {
            base.SetLoadState(memento, mapper);

            Memento<AttackCommandSaveData>.SetWithAutoMapper(this, memento, mapper);

            Source = (Actor)GameWorld.GameObjects[memento.State.SourceId];
            Target = (Actor)GameWorld.GameObjects[memento.State.TargetId];
        }

        protected override CommandResult ExecuteInternal()
        {
            _damage = Source.BasicAttack.Roll();

            Target.Health -= _damage;

            var message = $"{Source.Name} hit {Target.TargetedName}";

            return Result(CommandResult.Success(message));
        }

        protected override void UndoInternal()
        {
            Target.Health += _damage;
        }
    }
}
