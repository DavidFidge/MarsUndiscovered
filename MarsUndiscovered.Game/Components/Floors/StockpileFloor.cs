using Microsoft.Xna.Framework;

namespace MarsUndiscovered.Game.Components;

public class StockpileFloor : FloorType
{
    private char _asciiCharacter = (char)0xb0;
    private Color _foregroundColour = new Color(0xFF8B482A);
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