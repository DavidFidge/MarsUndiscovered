﻿
using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using MarsUndiscovered.Components;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Commands
{
    public class MeleeAttackCommand : BaseAttackCommand<MeleeAttackCommandSaveData>
    {
        public Actor Source { get; private set; }
        public Actor Target { get; private set; }
        
        private int _damage;

        public MeleeAttackCommand(IGameWorld gameWorld) : base(gameWorld)
        {
        }

        public void Initialise(Actor source, Actor target)
        {
            Source = source;
            Target = target;
        }

        public override IMemento<MeleeAttackCommandSaveData> GetSaveState()
        {
            var memento = new Memento<MeleeAttackCommandSaveData>(new MeleeAttackCommandSaveData());
            base.PopulateSaveState(memento.State);

            memento.State.SourceId = Source.ID;
            memento.State.TargetId = Target.ID;

            return memento;
        }

        public override void SetLoadState(IMemento<MeleeAttackCommandSaveData> memento)
        {
            base.PopulateLoadState(memento.State);

            Source = (Actor)GameWorld.GameObjects[memento.State.SourceId];
            Target = (Actor)GameWorld.GameObjects[memento.State.TargetId];
        }

        protected override CommandResult ExecuteInternal()
        {
            // Result does not need to be persisted as long as random number seed is the same
            _damage = Source.MeleeAttack.Roll();

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