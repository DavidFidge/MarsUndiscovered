namespace MarsUndiscovered.Game.Components;

public abstract class FloorType : GameObjectType
{
    public static Dictionary<string, FloorType> FloorTypes;

    public static MiningFacilityFloor MiningFacilityFloor = new MiningFacilityFloor();
    public static RockFloor RockFloor = new RockFloor();
    public static StockpileFloor StockpileFloor = new StockpileFloor();

    public override string Name
    {
        get => GetType().Name;
        set { }
    }

    static FloorType()
    {
        FloorTypes = new Dictionary<string, FloorType>();

        FloorTypes.Add(MiningFacilityFloor.Name, MiningFacilityFloor);
        FloorTypes.Add(RockFloor.Name, RockFloor);
        FloorTypes.Add(StockpileFloor.Name, StockpileFloor);
    }
}