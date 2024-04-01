namespace MarsUndiscovered.Game.Components.Dto;

public class MessagesStatus
{
    public List<string> Messages { get; } = new();
    public int SeenMessageCount { get; private set; }

    public void AddMessages(params string[] messages)
    {
        Messages.AddRange(messages);
    }
    
    public void AddMessages(IList<string> messages)
    {
        Messages.AddRange(messages);
    }

    public IList<string> GetUnprocessedMessages()
    {
        var unprocessedMessages = Messages.Skip(SeenMessageCount).ToList();
        SeenMessageCount = Messages.Count;
        return unprocessedMessages;
    }
}