using Microsoft.Xna.Framework;

namespace MarsUndiscovered.Game.Components;

public class StockpileWall : WallType
{
    public StockpileWall()
    {
        ForegroundColour = Color.Black;
        BackgroundColour = new Color(0xFF244BB6);
        AsciiCharacter = '#';
        IsTransparent = true;
    }
}