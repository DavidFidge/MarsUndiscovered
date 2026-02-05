namespace MarsUndiscovered.Game.Components;

public abstract class FloorType : GameObjectType
{
    public static Dictionary<string, FloorType> FloorTypes;

    public static BlankFloor BlankFloor = new BlankFloor();
    public static MiningFacilityFloor MiningFacilityFloor = new MiningFacilityFloor();
    public static AlienFloor AlienFloor = new AlienFloor();
    public static RockFloor RockFloor = new RockFloor();
    public static StockpileFloor StockpileFloor = new StockpileFloor();

    static FloorType()
    {
        FloorTypes = new Dictionary<string, FloorType>();

        FloorTypes.Add(BlankFloor.Name, BlankFloor);
        FloorTypes.Add(MiningFacilityFloor.Name, MiningFacilityFloor);
        FloorTypes.Add(AlienFloor.Name, AlienFloor);
        FloorTypes.Add(RockFloor.Name, RockFloor);
        FloorTypes.Add(StockpileFloor.Name, StockpileFloor);
    }
}