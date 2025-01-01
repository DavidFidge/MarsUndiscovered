using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Commands
{
    public class MeleeAttackCommand : BaseAttackCommand<MeleeAttackCommandSaveData>
    {
        public Actor Source => GameWorld.GameObjects[_data.SourceId] as Actor;
        public Actor Target => GameWorld.GameObjects[_data.TargetId] as Actor;
        public Item Item => _data.ItemId == null ? null : GameWorld.GameObjects[_data.ItemId.Value] as Item;

        public MeleeAttackCommand(IGameWorld gameWorld) : base(gameWorld)
        {
        }

        public void Initialise(Actor source, Actor target, Item item)
        {
            _data.SourceId = source.ID;
            _data.TargetId = target.ID;
            _data.ItemId = item?.ID;
        }

        protected override CommandResult ExecuteInternal()
        {
            if (Item != null && Source is Player player)
                player.RecalculateAttacksForItem(Item);

            var damage = Source.MeleeAttack.Roll();

            _data.AttackRestoreData = new AttackRestoreData
            {
                Damage = damage,
                Health = Target.Health,
                Shield = Target.Shield
            };
            
            ApplyWeaknesses(Source, Target);

            Target.ApplyDamage(damage);

            var message = $"{Source.GetSentenceName(false, false)} hit {Target.GetSentenceName(true, false)}";

            var commandResult = CommandResult.Success(this, message);
            
            SetHuntingIfAttackedByPlayer(Source, Target);

            if (Target.Health <= 0)
            {
                var deathCommand = CommandCollection.CreateCommand<DeathCommand>(GameWorld);
                deathCommand.Initialise(Target, Source.GetSentenceName(true, true));
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
