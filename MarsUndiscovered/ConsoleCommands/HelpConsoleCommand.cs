using System.Diagnostics.Eventing.Reader;
using System.Text;
using Castle.Core.Internal;

using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.ConsoleCommands;
using FrigidRogue.MonoGame.Core.Extensions;
using FrigidRogue.MonoGame.Core.Interfaces.ConsoleCommands;

namespace MarsUndiscovered.ConsoleCommands
{
    [ConsoleCommand(Name = "Help", Parameter1 = "CommandName")]
    public class HelpConsoleCommand : BaseConsoleCommand
    {
        private readonly Dictionary<string, IConsoleCommand> _consoleCommands;

        public HelpConsoleCommand(IConsoleCommand[] consoleCommands)
        {
            _consoleCommands = consoleCommands
                .ToDictionary(k => 
                    k.GetType().GetAttributes<ConsoleCommandAttribute>().Single().Name.ToLower(),
                    v => v);
        }

        public override void Execute(ConsoleCommand consoleCommand)
        {
            if (consoleCommand.Params.Any())
            {
                _consoleCommands.TryGetValue(consoleCommand.Params.First().ToLower(), out var command);

                if (command != null)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine($"Command: {command.GetType().GetAttributes<ConsoleCommandAttribute>().Single().Name}");
                    
                    sb.AppendLine($"Parameters: {command.GetType().GetAttributes<ConsoleCommandAttribute>().Single().Parameter1} {command.GetType().GetAttributes<ConsoleCommandAttribute>().Single().Parameter2}");

                    consoleCommand.Result = sb.ToString();
                }
            }
            else
            {
                var commandNames = _consoleCommands.Values
                    .Select(v => v.GetType().GetAttributes<ConsoleCommandAttribute>().Single().Name).ToCsv();
                
                consoleCommand.Result = $"List of commands (case insensitive): {commandNames}";
            }
        }
    }
}
