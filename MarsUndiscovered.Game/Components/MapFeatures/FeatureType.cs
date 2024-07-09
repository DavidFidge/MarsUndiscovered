namespace MarsUndiscovered.Game.Components;

public abstract class FeatureType : GameObjectType
{
    public static Dictionary<string, FeatureType> FeatureTypes;

    public static RubbleType RubbleType = new RubbleType();
    
    static FeatureType()
    {
        FeatureTypes = new Dictionary<string, FeatureType>();

        FeatureTypes.Add(RubbleType.Name, RubbleType);
    }

    public virtual string GetAmbientText()
    {
        return string.Empty;
    }
}