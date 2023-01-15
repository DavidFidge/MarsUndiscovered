namespace MarsUndiscovered.Components.Dto
{
    public class MessagesStatus
    {
        public List<string> Messages { get; private set; } = new List<string>();
        public int SeenMessageCount { get; private set; }
        public int ProcessedMessageCount { get; set; }

        public void AddMessages(IList<string> messages)
        {
            Messages.AddRange(messages);
            SeenMessageCount += Messages.Count;
        }

        public IEnumerable<string> GetUnprocessedMessages()
        {
            return Messages.Skip(ProcessedMessageCount);
        }

        public void SetSeenAllMessages()
        {
            ProcessedMessageCount = SeenMessageCount;
        }
    }
}