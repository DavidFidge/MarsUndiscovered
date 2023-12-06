using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Game.Commands
{
    public class UndoCommandSaveData : BaseCommandSaveData
    {
        public uint CommandId { get; set; }
    }
}
