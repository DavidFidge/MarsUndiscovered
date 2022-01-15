using System.Collections.Generic;
using System.Linq;

using Castle.Core.Internal;

using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.ConsoleCommands;
using FrigidRogue.MonoGame.Core.Extensions;
using FrigidRogue.MonoGame.Core.Interfaces.ConsoleCommands;

namespace MarsUndiscovered.ConsoleCommands
{
    [ConsoleCommand(Name = "Help")]
    public class HelpConsoleCommand : BaseConsoleCommand
    {
        private readonly List<string> _consoleCommandNames;

        public HelpConsoleCommand(IConsoleCommand[] consoleCommands)
        {
            _consoleCommandNames = consoleCommands
                .Select(c => c.GetType().GetAttributes<ConsoleCommandAttribute>().Single().Name)
                .ToList();
        }

        public override void Execute(ConsoleCommand consoleCommand)
        {
            consoleCommand.Result = $"List of commands (case insensitive): {_consoleCommandNames.ToCsv()}";
        }
    }
}
