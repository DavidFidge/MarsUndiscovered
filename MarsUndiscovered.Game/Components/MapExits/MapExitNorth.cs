
using Microsoft.Xna.Framework;

namespace MarsUndiscovered.Game.Components;

public class MapExitNorth : MapExitType
{
    public MapExitNorth()
    {
        AsciiCharacter = (char)0x1E;
        ForegroundColour = Color.Yellow;
        BackgroundColour = Color.Black;
    }

    public override string ExitText => "I head north";
    public override string HoverText => "An exit north";
    public override string DirectionText => "North";
}