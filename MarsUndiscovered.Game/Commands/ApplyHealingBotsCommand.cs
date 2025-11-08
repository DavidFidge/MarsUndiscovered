using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Commands
{
    public class ApplyHealingBotsCommand : BaseMarsGameActionCommand
    {
        public Item Source { get; set; }
        public Actor Target { get; set; }

        public ApplyHealingBotsCommand(IGameWorld gameWorld) : base(gameWorld)
        {
        }

        public void Initialise(Item source, Actor target)
        {
            Source = source;
            Target = target;
        }

        protected override CommandResult ExecuteInternal()
        {
            Target.MaxHealth += Source.MaxHealthIncrease;
            Target.Health = Target.MaxHealth;

            var message = "I feel healthier and all my ailments are cured.";
            var commandResult = CommandResult.Success(this, message);

            return Result(commandResult);
        }
    }
}
