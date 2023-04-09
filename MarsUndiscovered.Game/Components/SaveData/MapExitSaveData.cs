using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Components.SaveData
{
    public class MapExitSaveData : IndestructibleSaveData
    {
        public uint DestinationId { get; set; }
        public Point LandingPosition { get; set; }
        public Direction Direction { get; set; }
    }
}
