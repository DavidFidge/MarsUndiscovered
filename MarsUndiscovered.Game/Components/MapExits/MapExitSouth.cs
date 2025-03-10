
using Microsoft.Xna.Framework;

namespace MarsUndiscovered.Game.Components;

public class MapExitSouth : MapExitType
{
    public MapExitSouth()
    {
        AsciiCharacter = (char)0x1F;
        ForegroundColour = Color.Yellow;
        BackgroundColour = Color.Black;
    }

    public override string ExitText  => "I head south";
    public override string HoverText => "An exit south";
    public override string DirectionText => "South";
}