using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Commands
{
    public class ApplyShieldCommand : BaseMarsGameActionCommand
    {
        public Item Source { get; set; }
        public Actor Target { get; set; }

        public ApplyShieldCommand(IGameWorld gameWorld) : base(gameWorld)
        {
        }

        public void Initialise(Item source, Actor target)
        {
            Source = source;
            Target = target;
        }

        protected override CommandResult ExecuteInternal()
        {
            var shieldAmount = (Source.DamageShieldPercentage * Target.MaxHealth) / 100;

            Target.Shield = shieldAmount;

            var message = $"A soft glow and rhythmic hum surrounds {Target.GetSentenceName(true, false)}";
            var commandResult = CommandResult.Success(this, message);

            return Result(commandResult);
        }
    }
}
