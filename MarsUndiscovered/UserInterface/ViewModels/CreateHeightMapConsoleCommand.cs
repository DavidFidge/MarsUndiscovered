using MarsUndiscovered.Interfaces;

using DavidFidge.MonoGame.Core.ConsoleCommands;
using DavidFidge.MonoGame.Core.Interfaces.ConsoleCommands;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    [ConsoleCommand(Name = "CreateHeightMap")]
    public class CreateHeightMapConsoleCommand : IConsoleCommand
    {
        private readonly IMarsUndiscoveredGameWorld _marsUndiscoveredGameWorld;

        public CreateHeightMapConsoleCommand(IMarsUndiscoveredGameWorld marsUndiscoveredGameWorld)
        {
            _marsUndiscoveredGameWorld = marsUndiscoveredGameWorld;
        }

        public void Execute(ConsoleCommand consoleCommand)
        {
            _marsUndiscoveredGameWorld.RecreateHeightMap();

            consoleCommand.Result = "Terrain created";
        }
    }
}
