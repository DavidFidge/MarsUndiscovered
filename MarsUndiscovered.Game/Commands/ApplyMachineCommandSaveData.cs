using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Game.Commands
{
    public class ApplyMachineCommandSaveData : BaseCommandSaveData
    {
        public uint MachineId { get; set; }
        public bool OldIsUsed { get; set; }
        public bool IsUsed { get; set; }
    }
}
