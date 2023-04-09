namespace MarsUndiscovered.Game.Components
{
    public enum MapType
    {
        Mine,
        Outdoor
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