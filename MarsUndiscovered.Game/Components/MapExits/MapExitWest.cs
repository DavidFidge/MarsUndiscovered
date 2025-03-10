
using Microsoft.Xna.Framework;

namespace MarsUndiscovered.Game.Components;

public class MapExitWest : MapExitType
{
    public MapExitWest()
    {
        AsciiCharacter = (char)0x11;
        ForegroundColour = Color.Yellow;
        BackgroundColour = Color.Black;
    }

    public override string ExitText => "I head wast";
    public override string HoverText => "An exit west";
    public override string DirectionText => "West";
}