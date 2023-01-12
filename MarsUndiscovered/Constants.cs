namespace MarsUndiscovered
{
    public class Constants
    {
        public const float UiIndentLevel1 = 20f;
        public const float UiIndentLevel2 = 40f;
        public const float TextInputMinimalHeight = 0.12f;
        public const float MiddlePanelWidth = 0.805f;

        // Layers
        public const int TerrainLayer = 0;
        public const int ActorLayer = 1;
        public const int ItemLayer = 2;
        public const int IndestructiblesLayer = 3;
        public const int OutdoorAreaBorder = 2;
        public const int ShipOffset = 2;

        // For rendering the map texture to a texture
        public const int TileWidth = 32;
        public const int TileHeight = 58;

        // Width and height of one quad when drawing all the triangles in 3D
        public const float TileQuadHeight = 1f;
        public const float TileQuadWidth = 0.55f;
    }
}
