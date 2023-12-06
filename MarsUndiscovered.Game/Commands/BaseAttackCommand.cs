using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Commands
{
    public abstract class BaseAttackCommand<T> : BaseMarsGameActionCommand<T>
        where T : BaseCommandSaveData, new()
    {
        protected BaseAttackCommand(IGameWorld gameWorld) : base(gameWorld)
        {
            InterruptsMovement = true;
        }
        
        protected void SetHuntingIfAttackedByPlayer(Actor source, Actor target)
        {
            if (target is Monster monster && source is Player)
            {
                monster.MonsterState = MonsterState.Hunting;
            }
        }
    }
}
