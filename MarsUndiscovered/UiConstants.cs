namespace MarsUndiscovered
{
    public class UiConstants
    {
        public const float UiIndentLevel1 = 20f;
        public const float UiIndentLevel2 = 40f;
        public const float TextInputMinimalHeight = 0.12f;
        public const float GameViewPanelWidth = 0.85f;
        public const float LeftPanelWidth = 0.15f;
        
        // 0 is a special value to designate 100% height of the parent.
        // Note - a value between 0f and 1f is a percentage of the parent. 
        public const float HeightOfParent = 0f;
        
        public const float GameViewPanelHeight = 0.8f;
        public const float BottomPanelHeight = 0.2f;
        
        // Defined in relation to bottom panel. Both are currently leaving room for the status bar.
        public const float MessageLogHeight = 0.9f;
        public const float RadioCommsPanelHeight = 0.9f;
        
        public const float HotBarHeight = 0.08f;

        public const int ShipOffset = 2;

        // For rendering the map texture to a texture
        public const int TileWidth = 64;
        public const int TileHeight = 64;

        // Width and height of one quad when drawing all the triangles in 3D
        public const float TileQuadHeight = 1f;
        public const float TileQuadWidth = 1f;

        public const float MapTileAnimationSeconds = 0.5f;
        
        // Action names
        public const string InventoryItemSelectionCycleRequestNext = "Next Inventory Selection";
        public const string InventoryItemSelectionCycleRequestPrevious = "Previous Inventory Selection";
    }
}
