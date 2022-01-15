using System;
using System.Collections.Generic;
using System.Text;

using AutoMapper;

using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using MarsUndiscovered.Components;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Commands
{
    public class AttackCommand : BaseMarsGameActionCommand<AttackCommandSaveData>
    {
        public Actor Source { get; private set; }
        public Actor Target { get; private set; }

        private int _damage;

        public AttackCommand(IGameWorld gameWorld) : base(gameWorld)
        {
        }

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

            var message = $"{Source.NameSpecificArticleUpperCase} hit {Target.NameSpecificArticleLowerCase}";

            var commandResult = CommandResult.Success(this, message);

            if (Target.Health <= 0)
            {
                var deathCommand = CommandFactory.CreateDeathCommand(GameWorld);
                deathCommand.Initialise(Target, Source.NameGenericArticleLowerCase);
                commandResult.SubsequentCommands.Add(deathCommand);
            }

            return Result(commandResult);
        }

        protected override void UndoInternal()
        {
            Target.Health += _damage;
        }
    }
}
