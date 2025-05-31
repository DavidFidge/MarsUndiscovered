
using Microsoft.Xna.Framework;

namespace MarsUndiscovered.Game.Components;

public class MissileTargetType : EnvironmentalEffectType
{
    public MissileTargetType()
    {
        ForegroundColour = Color.Red;
        BackgroundColour = Color.Transparent;
        
        AsciiCharacter = (char)0xe9;

        Damage = 50;
        Duration = 2;
    }

    public override string GetAmbientText()
    {
        return "A missile will explode here soon!";
    }
}