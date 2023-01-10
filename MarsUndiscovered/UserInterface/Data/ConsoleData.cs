using FrigidRogue.MonoGame.Core.ConsoleCommands;

namespace MarsUndiscovered.UserInterface.Data
{
    public class ConsoleData
    {
        public LinkedList<ConsoleCommand> LastCommands { get; set; } = new LinkedList<ConsoleCommand>();

        public string Command { get; set; }
    }
}
