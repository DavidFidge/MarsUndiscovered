using FrigidRogue.MonoGame.Core.Components;

namespace MarsUndiscovered.Game.Commands
{
    public abstract class BaseCommandSaveData
    {
        public TurnDetails TurnDetails { get; set; }
        public bool AdvanceSequenceNumber { get; set; }
    }
}