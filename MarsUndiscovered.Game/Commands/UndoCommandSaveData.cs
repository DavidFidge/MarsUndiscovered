using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Game.Commands
{
    public class UndoCommandSaveData : BaseCommandSaveData
    {
        public Guid CommandId { get; set; }
    }
}
