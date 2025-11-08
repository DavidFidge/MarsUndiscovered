using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Commands
{
    public class ApplyMachineCommand : BaseMarsGameActionCommand
    {
        public Machine Machine { get; set; }
        public bool OldIsUsed { get; private set; }
        public bool IsUsed { get; private set; }

        public ApplyMachineCommand(IGameWorld gameWorld) : base(gameWorld)
        {
            EndsPlayerTurn = false;
        }

        public void Initialise(Machine machine)
        {
            Machine = machine;
        }

        protected override CommandResult ExecuteInternal()
        {
            if (Machine.IsUsed)
            {
                return Result(CommandResult.NoMove(this, "The machine no longer has power and lies dormant."));
            }

            OldIsUsed = Machine.IsUsed;
            Machine.IsUsed = true;
            IsUsed = true;

            BaseGameActionCommand subsequentCommand = null;
            string message;

            switch (Machine.MachineType)
            {
                case Analyzer:
                {
                    RequiresPlayerInput = true;
                    EndsPlayerTurn = false;
                    message = "The analyzer machine has enough power to identify 1 item";
                    break;
                }
                
                default:
                    throw new Exception($"Apply for MachineType {Machine.MachineType} has not been implemented");
            }
            
            return Result(CommandResult.Success(this, message, subsequentCommand));
        }

        // Used when cancelling out of item choice
        protected override void UndoInternal()
        {
            Machine.IsUsed = OldIsUsed;
            IsUsed = OldIsUsed;
        }
    }
}
