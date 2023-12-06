namespace MarsUndiscovered.Game.Components;

public abstract class MachineType : GameObjectType
{
    public static Dictionary<string, MachineType> MachineTypes;

    public static Analyzer Analyzer = new Analyzer();

    static MachineType()
    {
        MachineTypes = new Dictionary<string, MachineType>();
        MachineTypes.Add(Analyzer.Name, Analyzer);
    }
    
    public virtual string GetDescription()
    {
        return "Description";
    }
    
    public virtual string GetLongDescription()
    {
        return "Long Description";
    }
}