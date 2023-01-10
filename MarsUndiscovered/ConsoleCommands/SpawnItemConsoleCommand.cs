using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.ConsoleCommands;
using FrigidRogue.MonoGame.Core.Extensions;

using MarsUndiscovered.Components;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.ConsoleCommands
{
    [ConsoleCommand(Name = "SpawnItem", Parameter1 = "ItemType")]
    public class SpawnItemConsoleCommand : BaseConsoleCommand
    {
        public IGameWorldConsoleCommandEndpoint GameWorldConsoleCommandEndpoint { get; set; }

        public SpawnItemConsoleCommand()
        {
        }

        public override void Execute(ConsoleCommand consoleCommand)
        {
            if (consoleCommand.Params.Any())
            {
                var itemType = consoleCommand.Params[0];

                if (!ItemType.ItemTypes.ContainsKey(itemType))
                {
                    consoleCommand.Result = $"Invalid item type {itemType}. Valid item types are {ItemType.ItemTypes.Keys.ToCsv()}.";
                    return;
                }

                GameWorldConsoleCommandEndpoint.SpawnItem(new SpawnItemParams().WithItemType(itemType));

                consoleCommand.Result = $"Spawned item {itemType}";

                return;
            }

            consoleCommand.Result = $"Required Parameter ItemType. Valid item types are {ItemType.ItemTypes.Keys.ToCsv()}.";
        }
    }
}
