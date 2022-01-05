using CommandLine;

namespace MarsUndiscovered
{
    public class Options
    {
        [Option('n', "NewGame", Required = false, HelpText = "Starts a new game immediately, skipping the title screen")]
        public bool NewGame { get; set; }
    }
}