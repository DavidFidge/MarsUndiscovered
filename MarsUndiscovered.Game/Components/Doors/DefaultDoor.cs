﻿
using Microsoft.Xna.Framework;

namespace MarsUndiscovered.Game.Components;

public class DefaultDoor : DoorType
{
    private char _asciiCharacter = '+';
    private Color _foregroundColour =  Color.Orange;
    private Color? _backgroundColour = Color.DarkRed;

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