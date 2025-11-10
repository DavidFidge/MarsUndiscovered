using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Commands
{
    public class MeleeAttackCommand : BaseAttackCommand
    {
        public Actor Source { get; set; }
        public Actor Target { get; set; }
        public Item Item { get; set; }

        public MeleeAttackCommand(IGameWorld gameWorld) : base(gameWorld)
        {
        }

        public void Initialise(Actor source, Actor target, Item item)
        {
            Source = source;
            Target = target;
            Item = item;
        }

        protected override CommandResult ExecuteInternal()
        {
            if (Item != null && Source is Player player)
                player.RecalculateAttacksForItem(Item);

            var damage = Source.MeleeAttack.Roll();

            ApplyWeaknesses(Source, Target);

            Target.ApplyDamage(damage);

            var message = $"{Source.GetSentenceName(false, false)} hit {Target.GetSentenceName(true, false)}";

            var commandResult = CommandResult.Success(this, message);
            
            SetHuntingIfAttackedByPlayer(Source, Target);

            if (Target.Health <= 0)
            {
                var deathCommand = GameWorld.CommandCollection.CreateCommand<DeathCommand>(GameWorld);
                deathCommand.Initialise(Target, Source.GetSentenceName(true, true));
                commandResult.SubsequentCommands.Add(deathCommand);
            }

            return Result(commandResult);
        }
    }
}
