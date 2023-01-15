using GoRogue.GameFramework;

namespace MarsUndiscovered.Components
{
    public class RadioCommsEntry
    {
        public string Message { get; private set; }
        public IGameObject GameObject { get; private set; }

        public RadioCommsEntry(string message, IGameObject gameObject)
        {
            Message = message;
            GameObject = gameObject;
        }
    }
}