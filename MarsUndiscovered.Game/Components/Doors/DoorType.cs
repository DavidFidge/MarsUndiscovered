namespace MarsUndiscovered.Game.Components;

public abstract class DoorType : GameObjectType
{
    public static Dictionary<string, DoorType> DoorTypes;

    public static DefaultDoor DefaultDoor = new DefaultDoor();
    public static DefaultDoorOpened DefaultDoorOpened = new DefaultDoorOpened();
    
    public static Dictionary<DoorType, DoorType> OpenClosedMapping;
    public static Dictionary<DoorType, DoorType> ClosedOpenMapping;

    static DoorType()
    {
        DoorTypes = new Dictionary<string, DoorType>();

        DoorTypes.Add(DefaultDoor.Name, DefaultDoor);
        DoorTypes.Add(DefaultDoorOpened.Name, DefaultDoorOpened);
        
        OpenClosedMapping = new Dictionary<DoorType, DoorType>();
        OpenClosedMapping.Add(DefaultDoorOpened, DefaultDoor);
        
        ClosedOpenMapping = new Dictionary<DoorType, DoorType>();
        ClosedOpenMapping.Add(DefaultDoor, DefaultDoorOpened);
    }
}