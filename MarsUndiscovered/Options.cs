using CommandLine;

namespace MarsUndiscovered
{
    public class Options
    {
        [Option('n', "NewGame", Required = false, HelpText = "Starts a new game immediately, skipping the title screen")]
        public bool NewGame { get; set; }
        
        [Option('w', "WorldBuilder", Required = false, HelpText = "Goes to world builder immediately, skipping the title screen")]
        public bool WorldBuilder { get; set; }
        
        [Option( "EnableCameraMovement", Required = false, HelpText = "Allows the camera to be moved")]
        public bool EnableCameraMovement { get; set; }
    }
}