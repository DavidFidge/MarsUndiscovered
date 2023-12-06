using Microsoft.Xna.Framework;

namespace MarsUndiscovered.Game.Components;

public class MiningFacilityWall : WallType
{
    public MiningFacilityWall()
    {
        ForegroundColour = Color.Black;
        BackgroundColour = new Color(0xFF777777);
        AsciiCharacter = '#';
    }
}