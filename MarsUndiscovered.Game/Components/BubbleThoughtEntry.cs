namespace MarsUndiscovered.Game.Components;

public class BubbleThoughtEntry
{
    public BubbleThoughtTypes BubbleThoughtType { get; private set; }
    public string Message { get; private set; }
    public bool HasBeenSeen { get; set; }

    public BubbleThoughtEntry(BubbleThoughtTypes bubbleThoughtType, string message)
    {
        BubbleThoughtType = bubbleThoughtType;
        Message = message;
        HasBeenSeen = false;
    }
}