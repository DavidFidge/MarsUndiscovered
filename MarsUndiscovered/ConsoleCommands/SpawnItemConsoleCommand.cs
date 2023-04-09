using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.ConsoleCommands;
using FrigidRogue.MonoGame.Core.Extensions;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.ConsoleCommands
{
    [ConsoleCommand(Name = "SpawnItem", Parameter1 = "ItemType", Parameter2 = "[IntoPlayerInventory(true|false)]")]
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
                
                var spawnItemParams = new SpawnItemParams().WithItemType(itemType);

                if (consoleCommand.Params.Count > 1)
                {
                    var intoPlayerInventory = consoleCommand.Params[1];

                    if (Boolean.TryParse(intoPlayerInventory, out var intoPlayerInventoryResult))
                    {
                        spawnItemParams.IntoPlayerInventory(intoPlayerInventoryResult);
                    }
                    else
                    {
                        consoleCommand.Result = "Invalid IntoPlayerInventoryResult value. Valid values are true or false.";
                    }
                }

                GameWorldConsoleCommandEndpoint.SpawnItem(spawnItemParams);

                consoleCommand.Result = $"Spawned item {itemType}";

                return;
            }

            consoleCommand.Result = $"Required Parameter ItemType. Valid item types are {ItemType.ItemTypes.Keys.ToCsv()}.";
        }
    }
}
