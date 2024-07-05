
using Microsoft.Xna.Framework;

namespace MarsUndiscovered.Game.Components;

public class RubbleType : FeatureType
{
    public RubbleType()
    {
        ForegroundColour = Color.SaddleBrown;
        BackgroundColour = Color.Black;
        
        AsciiCharacter = (char)0xb0;
    }
}