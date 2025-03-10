
using Microsoft.Xna.Framework;

namespace MarsUndiscovered.Game.Components;

public class MapExitUp : MapExitType
{
    public MapExitUp()
    {
        AsciiCharacter = (char)0x18;
        ForegroundColour = Color.Yellow;
        BackgroundColour = Color.SaddleBrown;
    }

    public override string ExitText => "I ascend";
    public override string HoverText => "A passageway up";
    public override string DirectionText => "Up";
}