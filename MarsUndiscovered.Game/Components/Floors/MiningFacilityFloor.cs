using Microsoft.Xna.Framework;

namespace MarsUndiscovered.Game.Components;

public class MiningFacilityFloor : FloorType
{
    private char _asciiCharacter = (char)0xdb;
    private Color _foregroundColour =  new Color(0xFFB4B4B4);
    private Color? _backgroundColour = null;

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