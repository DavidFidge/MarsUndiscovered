using Microsoft.Xna.Framework;

namespace MarsUndiscovered.Game.Components;

public abstract class GameObjectType
{
    public abstract char AsciiCharacter { get; set; }
    public abstract Color ForegroundColour { get; set; }
    public abstract Color? BackgroundColour { get; set; }

    public abstract string Name { get; set; }
}