using Microsoft.Xna.Framework;

namespace MarsUndiscovered.Game.Components;

public class RockWall : WallType
{
    public RockWall()
    {
        ForegroundColour = new Color(0xFF244BB6);
        BackgroundColour = null;
        AsciiCharacter = (char)0xb2;
    }
}