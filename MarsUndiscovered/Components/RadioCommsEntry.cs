using GoRogue.GameFramework;

namespace MarsUndiscovered.Components
{
    public class RadioCommsEntry
    {
        public string Message { get; private set; }
        public string Source { get; private set; }
        public IGameObject GameObject { get; private set; }

        public RadioCommsEntry(string message, string source, IGameObject gameObject)
        {
            Message = message;
            Source = source;
            GameObject = gameObject;
        }
    }
}