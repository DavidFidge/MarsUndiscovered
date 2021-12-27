using System.Collections.Generic;

namespace MarsUndiscovered.Components
{
    public class MessageLog : List<MessageLogEntry>
    {
        public void AddMessage(string message)
        {
            Add(new MessageLogEntry(message));
        }
    }
}