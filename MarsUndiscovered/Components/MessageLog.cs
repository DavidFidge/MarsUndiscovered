using System;
using System.Collections.Generic;
using System.Linq;

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