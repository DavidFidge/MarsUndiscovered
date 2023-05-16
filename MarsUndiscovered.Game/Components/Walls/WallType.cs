using Microsoft.Xna.Framework;

namespace MarsUndiscovered.Game.Components;

public abstract class WallType : GameObjectType
{
    public static Dictionary<string, WallType> WallTypes;

    public static MiningFacilityWall MiningFacilityWall = new MiningFacilityWall();
    public static RockWall RockWall = new RockWall();
    public static StockpileWall StockpileWall = new StockpileWall();

    public override string Name
    {
        get => GetType().Name;
        set { }
    }

    static WallType()
    {
        WallTypes = new Dictionary<string, WallType>();

        WallTypes.Add(MiningFacilityWall.Name, MiningFacilityWall);
        WallTypes.Add(RockWall.Name, RockWall);
        WallTypes.Add(StockpileWall.Name, StockpileWall);
    }
}