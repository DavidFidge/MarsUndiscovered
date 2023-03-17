using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using MarsUndiscovered.Components;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Commands
{
    public class ApplyShieldCommand : BaseAttackCommand<ApplyShieldCommandSaveData>
    {
        public Item Source { get; private set; }
        public Actor Target { get; private set; }
        
        private int _oldShieldAmount;

        public ApplyShieldCommand(IGameWorld gameWorld) : base(gameWorld)
        {
        }

        public void Initialise(Item source, Actor target)
        {
            Source = source;
            Target = target;
        }

        public override IMemento<ApplyShieldCommandSaveData> GetSaveState()
        {
            var memento = new Memento<ApplyShieldCommandSaveData>(new ApplyShieldCommandSaveData());
            base.PopulateSaveState(memento.State);

            memento.State.SourceId = Source.ID;
            memento.State.TargetId = Target.ID;
            memento.State.OldShieldAmount = _oldShieldAmount;

            return memento;
        }

        public override void SetLoadState(IMemento<ApplyShieldCommandSaveData> memento)
        {
            base.PopulateLoadState(memento.State);

            Source = (Item)GameWorld.GameObjects[memento.State.SourceId];
            Target = (Actor)GameWorld.GameObjects[memento.State.TargetId];
            _oldShieldAmount = memento.State.OldShieldAmount;
        }

        protected override CommandResult ExecuteInternal()
        {
            var shieldAmount = (Source.DamageShieldPercentage * Target.MaxHealth) / 100;

            Target.Shield = shieldAmount;

            var message = $"A soft glow and rhythmic hum surrounds {Target.NameSpecificArticleLowerCase}";
            var commandResult = CommandResult.Success(this, message);

            return Result(commandResult);
        }

        protected override void UndoInternal()
        {
            Target.Shield = _oldShieldAmount;
        }
    }
}
