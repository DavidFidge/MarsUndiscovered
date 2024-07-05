using Microsoft.Xna.Framework;

namespace MarsUndiscovered.Game.Components;

public class StockpileFloor : FloorType
{
    public StockpileFloor()
    {
        AsciiCharacter = (char)0xb2;
        ForegroundColour = new Color(0xFF8B482A);
        BackgroundColour = null;
    }
}