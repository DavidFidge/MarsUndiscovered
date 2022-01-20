namespace MarsUndiscovered.Commands
{
    public class ChangeMapSaveData : BaseCommandSaveData
    {
        public uint GameObjectId { get; set; }
        public uint MapExitId { get; set; }
    }
}
