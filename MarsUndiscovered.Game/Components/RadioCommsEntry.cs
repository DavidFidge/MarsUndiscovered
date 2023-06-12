using GoRogue.GameFramework;

namespace MarsUndiscovered.Game.Components;

public class RadioCommsEntry
{
    public RadioCommsTypes RadioCommsType { get; private set; }
    public string Message { get; private set; }
    public string Source { get; private set; }
    public IGameObject GameObject { get; private set; }

    public RadioCommsEntry(RadioCommsTypes radioCommsType, IGameObject gameObject, string message, string source)
    {
        RadioCommsType = radioCommsType;
        Message = message;
        Source = source;
        GameObject = gameObject;
    }
}