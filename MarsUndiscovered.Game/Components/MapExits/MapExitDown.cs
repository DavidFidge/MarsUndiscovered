
using Microsoft.Xna.Framework;

namespace MarsUndiscovered.Game.Components;

public class MapExitDown : MapExitType
{
    public MapExitDown()
    {
        AsciiCharacter = (char)0x19;
        ForegroundColour = Color.Yellow;
        BackgroundColour = Color.SaddleBrown;
    }

    public override string ExitText => "I descend";
    public override string HoverText => "A passageway down";
    public override string DirectionText => "Down";
}