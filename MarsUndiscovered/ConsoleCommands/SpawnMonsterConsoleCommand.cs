using System;
using System.Collections.Generic;
using System.Linq;

using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.ConsoleCommands;

using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.ConsoleCommands
{
    [ConsoleCommand(Name = "SpawnMonster", Parameter1 = "Breed")]
    public class SpawnMonsterConsoleCommand : BaseConsoleCommand
    {
        public IGameWorldProvider GameWorldProvider { get; set; }

        public SpawnMonsterConsoleCommand()
        {
        }

        public override void Execute(ConsoleCommand consoleCommand)
        {
            if (consoleCommand.Params.Any())
            {
                var breed = consoleCommand.Params[0];

                GameWorldProvider.GameWorld.SpawnMonster(breed);

                consoleCommand.Result = $"Spawned monster {breed}";

                return;
            }

            consoleCommand.Result = "Required Parameter Breed";
        }
    }
}
