using System.Linq;

using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.ConsoleCommands;

using MarsUndiscovered.Messages;

namespace MarsUndiscovered.ConsoleCommands
{
    [ConsoleCommand(Name = "NewGame")]
    public class NewGameConsoleCommand : BaseConsoleCommand
    {
        public NewGameConsoleCommand()
        {
        }

        public override void Execute(ConsoleCommand consoleCommand)
        {
            uint? seed = null;
            if (consoleCommand.Params.Any())
            {
                var seedString = consoleCommand.Params[0];

                if (uint.TryParse(seedString, out var result))
                    seed = result;
            }

            Mediator.Send(new NewGameRequest { Seed = seed });

            consoleCommand.Result = "New Game created";
        }
    }
}
