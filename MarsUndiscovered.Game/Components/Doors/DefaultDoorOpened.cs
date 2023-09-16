
using Microsoft.Xna.Framework;

namespace MarsUndiscovered.Game.Components;

public class DefaultDoorOpened : DoorType
{
    public DefaultDoorOpened()
    {
        AsciiCharacter = ' ';
        ForegroundColour = Color.Transparent;
        BackgroundColour = Color.DarkRed;
    }
}