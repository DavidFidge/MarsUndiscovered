
using Microsoft.Xna.Framework;

namespace MarsUndiscovered.Game.Components;

public class DefaultDoor : DoorType
{
    public DefaultDoor()
    {
        AsciiCharacter = '+';
        ForegroundColour = Color.Orange;
        BackgroundColour = Color.DarkRed;
    }
}