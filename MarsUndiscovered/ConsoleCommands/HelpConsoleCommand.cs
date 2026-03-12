using System.Reflection;
using System.Text;

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
                    k.GetType().GetCustomAttributes<ConsoleCommandAttribute>().Single().Name.ToLower(),
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
                    sb.AppendLine($"Command: {command.GetType().GetCustomAttributes<ConsoleCommandAttribute>().Single().Name}");
                    
                    sb.AppendLine($"Parameters: {command.GetType().GetCustomAttributes<ConsoleCommandAttribute>().Single().Parameter1} {command.GetType().GetCustomAttributes<ConsoleCommandAttribute>().Single().Parameter2}");

                    consoleCommand.Result = sb.ToString();
                }
            }
            else
            {
                var commandNames = _consoleCommands.Values
                    .Select(v => v.GetType().GetCustomAttributes<ConsoleCommandAttribute>().Single().Name).ToCsv();
                
                consoleCommand.Result = $"List of commands (case insensitive): {commandNames}";
            }
        }
    }
}
