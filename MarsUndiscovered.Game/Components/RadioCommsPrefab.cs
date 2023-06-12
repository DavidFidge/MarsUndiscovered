namespace MarsUndiscovered.Game.Components;

public class RadioCommsPrefab
{
    public RadioCommsTypes RadioCommsType { get; }
    public string Message { get; }
    public string Source { get; }

    public RadioCommsPrefab(RadioCommsTypes radioCommsType, string source, string message)
    {
        Source = source;
        Message = message;
        RadioCommsType = radioCommsType;
    }
}