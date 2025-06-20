using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.ConsoleCommands;
using FrigidRogue.MonoGame.Core.Extensions;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.ConsoleCommands
{
    [ConsoleCommand(Name = "SpawnMonster", Parameter1 = "Breed")]
    public class SpawnMonsterConsoleCommand : BaseConsoleCommand
    {
        public IGameWorld GameWorld { get; set; }

        public override void Execute(ConsoleCommand consoleCommand)
        {
            if (consoleCommand.Params.Any())
            {
                var breed = consoleCommand.Params[0];

                if (!Breed.Breeds.ContainsKey(breed))
                {
                    consoleCommand.Result = $"Invalid breed {breed}. Valid breeds are {Breed.Breeds.Keys.ToCsv()}.";
                    return;

                }

                GameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed(breed));
                consoleCommand.Result = $"Spawned monster {breed}";
                return;
            }

            consoleCommand.Result = $"Required Parameter Breed. Valid breeds are {Breed.Breeds.Keys.ToCsv()}.";
        }
    }
}
