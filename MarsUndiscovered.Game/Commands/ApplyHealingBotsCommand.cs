using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Commands
{
    public class ApplyHealingBotsCommand : BaseAttackCommand<ApplyHealingBotsCommandSaveData>
    {
        public Item Source { get; private set; }
        public Actor Target { get; private set; }
        
        private int _oldHealth;
        private int _oldMaxHealth;

        public ApplyHealingBotsCommand(IGameWorld gameWorld) : base(gameWorld)
        {
        }

        public void Initialise(Item source, Actor target)
        {
            Source = source;
            Target = target;
        }

        public override IMemento<ApplyHealingBotsCommandSaveData> GetSaveState()
        {
            var memento = new Memento<ApplyHealingBotsCommandSaveData>(new ApplyHealingBotsCommandSaveData());
            base.PopulateSaveState(memento.State);

            memento.State.SourceId = Source.ID;
            memento.State.TargetId = Target.ID;
            memento.State.OldHealth = _oldHealth;
            memento.State.OldMaxHealth = _oldMaxHealth;

            return memento;
        }

        public override void SetLoadState(IMemento<ApplyHealingBotsCommandSaveData> memento)
        {
            base.PopulateLoadState(memento.State);

            Source = (Item)GameWorld.GameObjects[memento.State.SourceId];
            Target = (Actor)GameWorld.GameObjects[memento.State.TargetId];
            _oldHealth = memento.State.OldHealth;
            _oldMaxHealth = memento.State.OldMaxHealth;
        }

        protected override CommandResult ExecuteInternal()
        {
            _oldMaxHealth = Target.MaxHealth;
            _oldHealth = Target.Health;
            
            Target.MaxHealth += Source.MaxHealthIncrease;
            Target.Health = Target.MaxHealth;

            var message = "You feel healthier. All your ailments are cured and your max health has increased.";
            var commandResult = CommandResult.Success(this, message);

            return Result(commandResult);
        }

        protected override void UndoInternal()
        {
            Target.Health = _oldHealth;
            Target.MaxHealth = _oldMaxHealth;
        }
    }
}
