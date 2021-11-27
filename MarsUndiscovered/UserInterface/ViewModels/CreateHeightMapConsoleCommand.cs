using Augmented.Interfaces;

using DavidFidge.MonoGame.Core.ConsoleCommands;
using DavidFidge.MonoGame.Core.Interfaces.ConsoleCommands;

namespace Augmented.UserInterface.ViewModels
{
    [ConsoleCommand(Name = "CreateHeightMap")]
    public class CreateHeightMapConsoleCommand : IConsoleCommand
    {
        private readonly IAugmentedGameWorld _augmentedGameWorld;

        public CreateHeightMapConsoleCommand(IAugmentedGameWorld augmentedGameWorld)
        {
            _augmentedGameWorld = augmentedGameWorld;
        }

        public void Execute(ConsoleCommand consoleCommand)
        {
            _augmentedGameWorld.RecreateHeightMap();

            consoleCommand.Result = "Terrain created";
        }
    }
}
