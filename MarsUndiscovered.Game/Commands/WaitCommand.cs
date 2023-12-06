using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Commands
{
    public class WaitCommand : BaseMarsGameActionCommand<WaitCommandSaveData>
    {
        public Actor Actor => GameWorld.GameObjects[_data.ActorId] as Actor;

        public WaitCommand(IGameWorld gameWorld) : base(gameWorld)
        {
            EndsPlayerTurn = true;
            PersistForReplay = true;
        }

        public void Initialise(Actor actor)
        {
            _data.ActorId = actor.ID;
        }

        protected override CommandResult ExecuteInternal()
        {
            return Result(CommandResult.Success(this));
        }

        protected override void UndoInternal()
        {
        }
    }
}
