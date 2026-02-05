using Microsoft.Xna.Framework;

namespace MarsUndiscovered.Game.Components;

public class AlienFloor : FloorType
{
    public AlienFloor()
    {
        AsciiCharacter = (char)0xdb;
        ForegroundColour = new Color(0xFF533e5d);
        BackgroundColour = null;
    }
}