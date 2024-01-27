namespace MarsUndiscovered
{
    public class Constants
    {
        public const float UiIndentLevel1 = 20f;
        public const float UiIndentLevel2 = 40f;
        public const float TextInputMinimalHeight = 0.12f;
        public const float GameViewPanelWidth = 0.85f;
        public const float LeftPanelWidth = 0.15f;
        
        public const float GameViewPanelHeight = 0.8f;
        public const float BottomPanelHeight = 0.2f;
        
        // Defined in relation to bottom panel. Both are currently leaving room for the status bar.
        public const float MessageLogHeight = 0.9f;
        public const float RadioCommsPanelHeight = 0.9f;

        // Layers
        public const int TerrainLayer = 0;
        public const int ActorLayer = 1;
        public const int ItemLayer = 2;
        public const int IndestructiblesLayer = 3;
        public const int OutdoorAreaBorder = 2;
        public const int ShipOffset = 2;

        // For rendering the map texture to a texture
        public const int TileWidth = 64;
        public const int TileHeight = 64;

        // Width and height of one quad when drawing all the triangles in 3D
        public const float TileQuadHeight = 1f;
        public const float TileQuadWidth = 1f;

        public const float MapTileAnimationTime = 0.5f;
    }
}
