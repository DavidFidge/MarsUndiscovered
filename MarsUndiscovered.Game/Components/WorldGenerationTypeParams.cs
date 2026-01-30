namespace MarsUndiscovered.Game.Components
{
    public enum MapType
    {
        Mine,
        MineWithHoleEntrance,
        Outdoor,
        MiningFacility,
        Prefab
    }
    
    public class WorldGenerationTypeParams
    {
        public MapType MapType { get; set; }

        public WorldGenerationTypeParams(MapType mapType)
        {
            MapType = mapType;
        }
    }
}