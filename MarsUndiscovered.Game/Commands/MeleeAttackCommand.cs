using FrigidRogue.MonoGame.Core.Components;

using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Commands
{
    public class MeleeAttackCommand : BaseAttackCommand<MeleeAttackCommandSaveData>
    {
        public Actor Source => GameWorld.GameObjects[_data.SourceId] as Actor;
        public Actor Target => GameWorld.GameObjects[_data.TargetId] as Actor;

        public MeleeAttackCommand(IGameWorld gameWorld) : base(gameWorld)
        {
        }

        public void Initialise(Actor source, Actor target)
        {
            _data.SourceId = source.ID;
            _data.TargetId = target.ID;
        }

        protected override CommandResult ExecuteInternal()
        {
            var damage = Source.MeleeAttack.Roll();

            _data.AttackRestoreData = new AttackRestoreData
            {
                Damage = damage,
                Health = Target.Health,
                Shield = Target.Shield
            };

            Target.ApplyDamage(damage);

            var message = $"{Source.NameSpecificArticleUpperCase} hit {Target.NameSpecificArticleLowerCase}";

            var commandResult = CommandResult.Success(this, message);
            
            SetHuntingIfAttackedByPlayer(Source, Target);

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
            Target.Health = _data.AttackRestoreData.Health;
            Target.Shield = _data.AttackRestoreData.Shield;
        }
    }
}
