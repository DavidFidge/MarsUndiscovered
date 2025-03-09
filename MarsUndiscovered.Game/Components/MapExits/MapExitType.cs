namespace MarsUndiscovered.Game.Components;

public abstract class MapExitType : GameObjectType
{
    public static Dictionary<string, MapExitType> MapExitTypes;

    public static MapExitUp MapExitUp = new MapExitUp();
    public static MapExitDown MapExitDown = new MapExitDown();
    public static MapExitNorth MapExitNorth = new MapExitNorth();
    public static MapExitSouth MapExitSouth = new MapExitSouth();
    public static MapExitEast MapExitEast = new MapExitEast();
    public static MapExitWest MapExitWest = new MapExitWest();
    
    static MapExitType()
    {
        MapExitTypes = new Dictionary<string, MapExitType>();

        MapExitTypes.Add(MapExitUp.Name, MapExitUp);
        MapExitTypes.Add(MapExitDown.Name, MapExitDown);
        MapExitTypes.Add(MapExitNorth.Name, MapExitNorth);
        MapExitTypes.Add(MapExitSouth.Name, MapExitSouth);
        MapExitTypes.Add(MapExitEast.Name, MapExitEast);
        MapExitTypes.Add(MapExitWest.Name, MapExitWest);
    }

    public abstract string ExitText { get; }
    public abstract string HoverText { get; }
    public abstract string DirectionText { get; }
}