
using Microsoft.Xna.Framework;

namespace MarsUndiscovered.Game.Components;

public class MissileTargetType : EnvironmentalEffectType
{
    public MissileTargetType()
    {
        ForegroundColour = Color.Red;
        BackgroundColour = Color.Transparent;
        
        AsciiCharacter = (char)0xe9;

        ForegroundColour2 = Color.Transparent;
        BackgroundColour2 = Color.Transparent;

        AsciiCharacter2 = (char)0x20;

        Damage = 50;
        Duration = 2;
    }

    public override string GetAmbientText()
    {
        return "A missile will explode here soon!";
    }
}