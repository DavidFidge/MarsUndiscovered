namespace MarsUndiscovered.Game.Components;

public abstract class EnvironmentalEffectType : GameObjectType
{
    public static Dictionary<string, EnvironmentalEffectType> EnvironmentalEffectTypes;

    public static MissileTargetType MissileTargetType = new MissileTargetType();
    
    public virtual int Damage { get; set; }
    public virtual int Duration { get; set; }
    
    static EnvironmentalEffectType()
    {
        EnvironmentalEffectTypes = new Dictionary<string, EnvironmentalEffectType>();

        EnvironmentalEffectTypes.Add(MissileTargetType.Name, MissileTargetType);
    }

    public virtual string GetAmbientText()
    {
        return string.Empty;
    }
}