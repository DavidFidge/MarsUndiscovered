
namespace Augmented.Graphics.TerrainSpace
{
    public class TerrainParameters
    {
       public WorldSize Size { get; set; }
       public HillHeight HillHeight { get; set; }

       public TerrainParameters(WorldSize size, HillHeight hillHeight)
       {
           Size = size;
           HillHeight = hillHeight;

       }
    }

    public enum WorldSize
    {
        Small = 0,
        Medium = 1,
        Big = 2
    }

    public enum HillHeight
    {
        Low = 0,
        Medium = 1,
        High = 2
    }
}