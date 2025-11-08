using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Commands
{
    public class UndoCommand : BaseMarsGameActionCommand
    {
        public BaseGameActionCommand Command { get; set; }

        public UndoCommand(IGameWorld gameWorld) : base(gameWorld)
        {
            EndsPlayerTurn = false;
        }

        public void Initialise(uint commandId)
        {
            Command = GameWorld.CommandCollection.GetCommand(commandId);
        }

        protected override CommandResult ExecuteInternal()
        {
            Command.Undo();
            
            return Result(CommandResult.Success(this));
        }

        protected override void UndoInternal()
        {
            Command.Execute();
        }
    }
}
