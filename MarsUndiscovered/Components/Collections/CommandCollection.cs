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
            DeathCommands = new DeathCommandCollection(commandFactory, gameWorld); 
            EquipItemCommands = new EquipItemCommandCollection(commandFactory, gameWorld);
            UnequipItemCommands = new UnequipItemCommandCollection(commandFactory, gameWorld);
            DropItemCommands = new DropItemCommandCollection(commandFactory, gameWorld);
            PickUpItemCommands = new PickUpItemCommandCollection(commandFactory, gameWorld);
        }

        public AttackCommandCollection AttackCommands { get; set; }
        public WalkCommandCollection WalkCommands { get; set; }
        public MoveCommandCollection MoveCommands { get; set; }
        public DeathCommandCollection DeathCommands { get; set; }
        public EquipItemCommandCollection EquipItemCommands { get; set; }
        public UnequipItemCommandCollection UnequipItemCommands { get; set; }
        public DropItemCommandCollection DropItemCommands { get; set; }
        public PickUpItemCommandCollection PickUpItemCommands { get; set; }

        public IEnumerator<BaseGameActionCommand> GetEnumerator()
        {
            foreach (var item in AttackCommands)
                yield return item;

            foreach (var item in WalkCommands)
                yield return item;

            foreach (var item in MoveCommands)
                yield return item;

            foreach (var item in DeathCommands)
                yield return item;

            foreach (var item in EquipItemCommands)
                yield return item;

            foreach (var item in UnequipItemCommands)
                yield return item;

            foreach (var item in PickUpItemCommands)
                yield return item;

            foreach (var item in DropItemCommands)
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
            DeathCommands.SaveState(saveGameService);
            EquipItemCommands.SaveState(saveGameService);
            UnequipItemCommands.SaveState(saveGameService);
            PickUpItemCommands.SaveState(saveGameService);
            DropItemCommands.SaveState(saveGameService);
        }

        public void LoadState(ISaveGameService saveGameService)
        {
            AttackCommands.LoadState(saveGameService);
            WalkCommands.LoadState(saveGameService);
            MoveCommands.LoadState(saveGameService);
            DeathCommands.LoadState(saveGameService);
            EquipItemCommands.LoadState(saveGameService);
            UnequipItemCommands.LoadState(saveGameService);
            PickUpItemCommands.LoadState(saveGameService);
            DropItemCommands.LoadState(saveGameService);
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
                case DeathCommand deathCommand:
                    DeathCommands.Add(deathCommand);
                    break;
                case EquipItemCommand equipItemCommand:
                    EquipItemCommands.Add(equipItemCommand);
                    break;
                case UnequipItemCommand unequipItemCommand:
                    UnequipItemCommands.Add(unequipItemCommand);
                    break;
                case PickUpItemCommand pickUpItemCommand:
                    PickUpItemCommands.Add(pickUpItemCommand);
                    break;
                case DropItemCommand dropItemCommand:
                    DropItemCommands.Add(dropItemCommand);
                    break;
            }
        }
    }
}