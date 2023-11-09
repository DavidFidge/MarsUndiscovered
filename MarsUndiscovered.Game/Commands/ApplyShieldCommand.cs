using FrigidRogue.MonoGame.Core.Components;

using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Commands
{
    public class ApplyShieldCommand : BaseMarsGameActionCommand<ApplyShieldCommandSaveData>
    {
        public Item Source => GameWorld.Items[_data.SourceId];
        public Actor Target => GameWorld.GameObjects[_data.TargetId] as Actor;
        
        public ApplyShieldCommand(IGameWorld gameWorld) : base(gameWorld)
        {
        }

        public void Initialise(Item source, Actor target)
        {
            _data.SourceId = source.ID;
            _data.TargetId = target.ID;
        }

        protected override CommandResult ExecuteInternal()
        {
            var shieldAmount = (Source.DamageShieldPercentage * Target.MaxHealth) / 100;

            _data.OldShieldAmount = Target.Shield;
            Target.Shield = shieldAmount;

            var message = $"A soft glow and rhythmic hum surrounds {Target.NameSpecificArticleLowerCase}";
            var commandResult = CommandResult.Success(this, message);

            return Result(commandResult);
        }

        protected override void UndoInternal()
        {
            Target.Shield = _data.OldShieldAmount;
        }
    }
}
