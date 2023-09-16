using Microsoft.Xna.Framework;

namespace MarsUndiscovered.Game.Components;

public class MiningFacilityFloor : FloorType
{
    public MiningFacilityFloor()
    {
        AsciiCharacter = (char)0xdb;
        ForegroundColour = new Color(0xFFB4B4B4);
        BackgroundColour = null;
    }
}