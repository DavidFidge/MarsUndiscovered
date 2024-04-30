namespace MarsUndiscovered.Game
{
    public class Constants
    {
        public const string GameVersion = "0.1.4";

        // Layers
        public const int TerrainLayer = 0;
        public const int DoorLayer = 1;
        public const int ActorLayer = 2;
        public const int ItemLayer = 3;
        public const int IndestructiblesLayer = 3; // same layer as items
        public const int OutdoorAreaBorder = 2;
        public const int ShipOffset = 2;
        
        public const char ConnectorPrefab = 'C';
        public const char WallDoNotTunnel = 'X';
        public const char WallPrefab = '#';
        public const char FloorPrefab = '.';
    }
}
