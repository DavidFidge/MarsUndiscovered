namespace MarsUndiscovered.Components;

public class RadioCommsPrefab
{
    public int Id { get; }
    public string Message { get; }
    public string Source { get; }

    public RadioCommsPrefab(int id, string source, string message)
    {
        Source = source;
        Message = message;
        Id = id;
    }
}