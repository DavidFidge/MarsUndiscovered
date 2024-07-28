using FrigidRogue.MonoGame.Core.Components;

using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Commands
{
    public class UndoCommand : BaseMarsGameActionCommand<UndoCommandSaveData>
    {
        public BaseGameActionCommand Command =>
            GameWorld.CommandCollection.GetCommand(_data.CommandId);
        
        public UndoCommand(IGameWorld gameWorld) : base(gameWorld)
        {
            EndsPlayerTurn = false;
        }

        public void Initialise(uint commandId)
        {
            _data.CommandId = commandId;
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
