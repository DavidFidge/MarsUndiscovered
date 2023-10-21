using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using GoRogue.GameFramework;

using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Commands
{
    public class ApplyMachineCommand : BaseMarsGameActionCommand<ApplyMachineCommandSaveData>
    {
        public Machine Machine { get; private set; }
        public bool IsUsed { get; private set; }
        
        public ApplyMachineCommand(IGameWorld gameWorld) : base(gameWorld)
        {
            EndsPlayerTurn = true;
            PersistForReplay = true;
        }

        public void Initialise(Machine machine)
        {
            Machine = machine;
        }

        public override IMemento<ApplyMachineCommandSaveData> GetSaveState()
        {
            var memento = new Memento<ApplyMachineCommandSaveData>(new ApplyMachineCommandSaveData());
            base.PopulateSaveState(memento.State);
            memento.State.MachineId = Machine.ID;
            memento.State.IsUsed = Machine.IsUsed;
            
            return memento;
        }

        public override void SetLoadState(IMemento<ApplyMachineCommandSaveData> memento)
        {
            base.PopulateLoadState(memento.State);
            Machine = GameWorld.Machines[memento.State.MachineId];
            IsUsed = memento.State.IsUsed;
        }

        protected override CommandResult ExecuteInternal()
        {
            if (Machine.IsUsed)
            {
                EndsPlayerTurn = false;
                return Result(CommandResult.NoMove(this, $"The machine no longer has power and lies dormant."));
            }

            Machine.IsUsed = true;

            BaseGameActionCommand subsequentCommand = null;

            switch (Machine.MachineType)
            {
                case Analyzer:
                {
                    RequiresPlayerInput = true;
                    EndsPlayerTurn = false;
                    break;
                }
                
                default:
                    throw new Exception($"Apply for MachineType {Machine.MachineType} has not been implemented");
            }
            
            return Result(CommandResult.Success(this, subsequentCommand));
        }

        protected override void UndoInternal()
        {
        }
    }
}
