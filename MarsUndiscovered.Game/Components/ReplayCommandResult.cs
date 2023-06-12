using FrigidRogue.MonoGame.Core.Components;

public class ReplayCommandResult
{
    public IList<CommandResult> CommandResults { get; }
    public bool HasMoreCommands { get; private set; }

    public ReplayCommandResult(IList<CommandResult> commandResults)
    {
        CommandResults = commandResults;
        HasMoreCommands = true;
    }

    public static ReplayCommandResult NoMoreCommands()
    {
        var result = new ReplayCommandResult(new List<CommandResult>())
        {
            HasMoreCommands = false
        };

        return result;
    }
}