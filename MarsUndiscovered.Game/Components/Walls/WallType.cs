namespace MarsUndiscovered.Game.Components;

public abstract class WallType : GameObjectType
{
    public static Dictionary<string, WallType> WallTypes;

    public static MiningFacilityWall MiningFacilityWall = new MiningFacilityWall();
    public static RockWall RockWall = new RockWall();
    public static StockpileWall StockpileWall = new StockpileWall();

    public bool IsTransparent { get; set; } = false;

    static WallType()
    {
        WallTypes = new Dictionary<string, WallType>();

        WallTypes.Add(MiningFacilityWall.Name, MiningFacilityWall);
        WallTypes.Add(RockWall.Name, RockWall);
        WallTypes.Add(StockpileWall.Name, StockpileWall);
    }
}