using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;
using MarsUndiscovered.Game.Components.SaveData;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components;

public class Machine : Indestructible, IMementoState<MachineSaveData>
{
    public override char AsciiCharacter => MachineType.AsciiCharacter;

    public MachineType MachineType { get; set; }
    public bool IsUsed { get; set; }
    
    public Machine(IGameWorld gameWorld, uint id) : base(gameWorld, id)
    {
    }
    
    public Machine WithMachineType(MachineType machineType)
    {
        MachineType = machineType;
        Description = machineType.GetLongDescription();

        return this;
    }

    public IMemento<MachineSaveData> GetSaveState()
    {
        var memento = new Memento<MachineSaveData>(new MachineSaveData());

        PopulateSaveState(memento.State);

        memento.State.MachineTypeName = MachineType.Name;
        memento.State.IsUsed = IsUsed;
        
        return memento;
    }

    public void SetLoadState(IMemento<MachineSaveData> memento)
    {
        PopulateLoadState(memento.State);
        IsUsed = memento.State.IsUsed;
        MachineType = MachineType.MachineTypes[memento.State.MachineTypeName];
    }
}