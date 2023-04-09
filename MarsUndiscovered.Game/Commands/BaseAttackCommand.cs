using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Commands
{
    public abstract class BaseAttackCommand<T> : BaseMarsGameActionCommand<T>
    {
        protected BaseAttackCommand(IGameWorld gameWorld) : base(gameWorld)
        {
        }

        public override bool InterruptsMovement => true;
    }
}
