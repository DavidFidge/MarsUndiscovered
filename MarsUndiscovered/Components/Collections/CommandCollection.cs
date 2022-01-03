using System.Collections;
using System.Collections.Generic;

using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using MarsUndiscovered.Commands;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Components
{
    public class CommandCollection : IEnumerable<BaseGameActionCommand>, ISaveable
    {
        public CommandCollection(ICommandFactory commandFactory, IGameWorld gameWorld)
        {
            AttackCommands = new AttackCommandCollection(commandFactory, gameWorld);
            WalkCommands = new WalkCommandCollection(commandFactory, gameWorld);
            MoveCommands = new MoveCommandCollection(commandFactory, gameWorld);
        }

        public AttackCommandCollection AttackCommands { get; set; }
        public WalkCommandCollection WalkCommands { get; set; }
        public MoveCommandCollection MoveCommands { get; set; }

        public IEnumerator<BaseGameActionCommand> GetEnumerator()
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

        public void SaveState(ISaveGameService saveGameService)
        {
            AttackCommands.SaveState(saveGameService);
            WalkCommands.SaveState(saveGameService);
            MoveCommands.SaveState(saveGameService);
        }

        public void LoadState(ISaveGameService saveGameService)
        {
            AttackCommands.LoadState(saveGameService);
            WalkCommands.LoadState(saveGameService);
            MoveCommands.LoadState(saveGameService);
        }

        public void AddCommand(BaseGameActionCommand command)
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