using System;
using System.Collections.Generic;
using System.Linq;

using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.ConsoleCommands;
using MarsUndiscovered.Components;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.ConsoleCommands
{
    [ConsoleCommand(Name = "SpawnItem", Parameter1 = "ItemType")]
    public class SpawnItemConsoleCommand : BaseConsoleCommand
    {
        public IGameWorldProvider GameWorldProvider { get; set; }

        public SpawnItemConsoleCommand()
        {
        }

        public override void Execute(ConsoleCommand consoleCommand)
        {
            if (consoleCommand.Params.Any())
            {
                var itemType = consoleCommand.Params[0];

                GameWorldProvider.GameWorld.SpawnItem(new SpawnItemParams().WithItemType(itemType));

                consoleCommand.Result = $"Spawned item {itemType}";

                return;
            }

            consoleCommand.Result = "Required Parameter ItemType";
        }
    }
}
