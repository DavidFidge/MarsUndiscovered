using System.Collections;
using System.Collections.Generic;

using System.Linq;

using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;

using MarsUndiscovered.Components.Factories;

namespace MarsUndiscovered.Components
{
    public class CommandCollection : IEnumerable<BaseCommand>, ISaveable
    {
        public CommandCollection(ICommandFactory commandFactory)
        {
            AttackCommands = new AttackCommandCollection(commandFactory);
            WalkCommands = new WalkCommandCollection(commandFactory);
            MoveCommands = new MoveCommandCollection(commandFactory);
        }

        public AttackCommandCollection AttackCommands { get; set; }
        public WalkCommandCollection WalkCommands { get; set; }
        public MoveCommandCollection MoveCommands { get; set; }

        public IEnumerator<BaseCommand> GetEnumerator()
        {
            foreach (var item in AttackCommands)
                yield return item;

            foreach (var item in WalkCommands)
                yield return item;

            foreach (var item in MoveCommands)
                yield return item;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void SaveState(ISaveGameStore saveGameStore)
        {
            AttackCommands.SaveState(saveGameStore);
            WalkCommands.SaveState(saveGameStore);
            MoveCommands.SaveState(saveGameStore);
        }

        public void LoadState(ISaveGameStore saveGameStore)
        {
            AttackCommands.LoadState(saveGameStore);
            WalkCommands.LoadState(saveGameStore);
            MoveCommands.LoadState(saveGameStore);
        }
    }
}