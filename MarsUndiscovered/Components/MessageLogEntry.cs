namespace MarsUndiscovered.Components
{
    public class MessageLogEntry
    {
        public string Message { get; private set; }

        public MessageLogEntry(string message)
        {
            Message = message;
        }
    }
}