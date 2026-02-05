using Microsoft.Xna.Framework;

namespace MarsUndiscovered.Game.Components;

public class AlienWall : WallType
{
    public AlienWall()
    {
        ForegroundColour = Color.Black;
        BackgroundColour = new Color(0xFF8f6f9f);
        AsciiCharacter = '#';
    }
}