
using Microsoft.Xna.Framework;

namespace MarsUndiscovered.Game.Components;

public class MapExitEast : MapExitType
{
    public MapExitEast()
    {
        AsciiCharacter = (char)0x10;
        ForegroundColour = Color.Yellow;
        BackgroundColour = Color.Black;
    }

    public override string ExitText  => "I head east";
    public override string HoverText => "An exit east";
    public override string DirectionText => "East";
}