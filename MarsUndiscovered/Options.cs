using CommandLine;

namespace MarsUndiscovered
{
    public class Options
    {
        [Option('n', "NewGame", Required = false, HelpText = "Starts a new game immediately, skipping the title screen")]
        public bool NewGame { get; set; }
        
        [Option( "SkipRadioComms", Required = false, HelpText = "Do not show radio comms messages")]
        public bool SkipRadioComms { get; set; }
        
        [Option('w', "WorldBuilder", Required = false, HelpText = "Goes to world builder immediately, skipping the title screen")]
        public bool WorldBuilder { get; set; }

        [Option( "WizardMode", Required = false, HelpText = "Start in wizard mode")]
        public bool WizardMode { get; set; }
    }
}