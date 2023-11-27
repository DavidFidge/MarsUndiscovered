using FrigidRogue.MonoGame.Core.Components;

namespace MarsUndiscovered.Game.Commands
{
    public abstract class BaseCommandSaveData : ICloneable
    {
        public Guid Id { get; set; }
        public TurnDetails TurnDetails { get; set; }
        public bool AdvanceSequenceNumber { get; set; }
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}