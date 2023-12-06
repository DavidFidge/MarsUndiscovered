
using Microsoft.Xna.Framework;

namespace MarsUndiscovered.Game.Components;

public class BlankFloor : FloorType
{
    public BlankFloor()
    {
        AsciiCharacter = ' ';
        ForegroundColour = Color.Black;
        BackgroundColour = null;
    }
}