using System.Collections;
using System.Collections.Generic;

using System.Linq;

using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using MarsUndiscovered.Commands;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Components
{
    public class CommandCollection : IEnumerable<BaseCommand>, ISaveable
    {
        private readonly IGameWorld _gameWorld;

        public CommandCollection(ICommandFactory commandFactory, IGameWorld gameWorld)
        {
            _gameWorld = gameWorld;
            AttackCommands = new AttackCommandCollection(commandFactory, gameWorld);
            WalkCommands = new WalkCommandCollection(commandFactory, gameWorld);
            MoveCommands = new MoveCommandCollection(commandFactory, gameWorld);
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

        public void AddCommand(BaseCommand command)
        {
            switch (command)
            {
                case AttackCommand attackCommand:
                    AttackCommands.Add(attackCommand);
                    break;
                case MoveCommand moveCommand:
                    MoveCommands.Add(moveCommand);
                    break;
                case WalkCommand walkCommand:
                    WalkCommands.Add(walkCommand);
                    break;
            }
        }
    }
}