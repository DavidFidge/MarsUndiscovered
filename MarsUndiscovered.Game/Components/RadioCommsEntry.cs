using GoRogue.GameFramework;

namespace MarsUndiscovered.Game.Components;

public class RadioCommsEntry
{
    public int Id { get; private set; }
    public string Message { get; private set; }
    public string Source { get; private set; }
    public IGameObject GameObject { get; private set; }

    public RadioCommsEntry(int id, string message, string source, IGameObject gameObject)
    {
        Id = id;
        Message = message;
        Source = source;
        GameObject = gameObject;
    }
}