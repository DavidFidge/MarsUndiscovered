using Microsoft.Xna.Framework;

namespace MarsUndiscovered.Game.Components;

public class RockFloor : FloorType
{
    public RockFloor()
    {
        AsciiCharacter = (char)0xfa;
        ForegroundColour = new Color(0xFF244BB6);
        BackgroundColour = null;
    }
}