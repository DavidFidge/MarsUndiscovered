using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Commands
{
    public class ApplyHealingBotsCommand : BaseMarsGameActionCommand<ApplyHealingBotsCommandSaveData>
    {
        public Item Source => GameWorld.Items[_data.SourceId];
        public Actor Target => GameWorld.GameObjects[_data.TargetId] as Actor;
        
        public ApplyHealingBotsCommand(IGameWorld gameWorld) : base(gameWorld)
        {
        }

        public void Initialise(Item source, Actor target)
        {
            _data.SourceId = source.ID;
            _data.TargetId = target.ID;
        }

        protected override CommandResult ExecuteInternal()
        {
            _data.OldMaxHealth = Target.MaxHealth;
            _data.OldHealth = Target.Health;
            
            Target.MaxHealth += Source.MaxHealthIncrease;
            Target.Health = Target.MaxHealth;

            var message = "I feel healthier and all my ailments are cured.";
            var commandResult = CommandResult.Success(this, message);

            return Result(commandResult);
        }
    }
}
