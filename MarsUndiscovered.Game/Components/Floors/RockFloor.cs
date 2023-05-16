using Microsoft.Xna.Framework;

namespace MarsUndiscovered.Game.Components;

public class RockFloor : FloorType
{
    private char _asciiCharacter = '#';
    private Color _foregroundColour = Color.White;
    private Color? _backgroundColour = new Color(0xFF244BB6);

    public override char AsciiCharacter
    {
        get => _asciiCharacter;
        set => _asciiCharacter = value;
    }

    public override Color ForegroundColour
    {
        get => _foregroundColour;
        set => _foregroundColour = value;
    }

    public override Color? BackgroundColour
    {
        get => _backgroundColour;
        set => _backgroundColour = value;
    }
}