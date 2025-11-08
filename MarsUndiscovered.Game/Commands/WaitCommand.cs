using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Commands
{
    public class WaitCommand : BaseMarsGameActionCommand
    {
        public Actor Actor { get; set; }

        public WaitCommand(IGameWorld gameWorld) : base(gameWorld)
        {
            EndsPlayerTurn = true;
        }

        public void Initialise(Actor actor)
        {
            Actor = actor;
        }

        protected override CommandResult ExecuteInternal()
        {
            return Result(CommandResult.Success(this));
        }
    }
}
