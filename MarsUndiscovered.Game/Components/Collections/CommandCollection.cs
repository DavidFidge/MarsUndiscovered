using System.Collections;
using System.Reflection;
using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components
{
    public class CommandCollection : ICommandCollection
    {
        private uint _nextId = 1;

        public ICommandFactory<LineAttackCommand> LineAttackCommandFactory { get; set; }
        public ICommandFactory<MeleeAttackCommand> MeleeAttackCommandFactory { get; set; }
        public ICommandFactory<LightningAttackCommand> LightningAttackCommandFactory { get; set; }
        public ICommandFactory<LaserAttackCommand> LaserAttackCommandFactory { get; set; }
        public ICommandFactory<WalkCommand> WalkCommandFactory { get; set; }
        public ICommandFactory<MoveCommand> MoveCommandFactory { get; set; }
        public ICommandFactory<DeathCommand> DeathCommandFactory { get; set; }
        public ICommandFactory<EquipItemCommand> EquipItemCommandFactory { get; set; }
        public ICommandFactory<UnequipItemCommand> UnequipItemCommandFactory { get; set; }
        public ICommandFactory<DropItemCommand> DropItemCommandFactory { get; set; }
        public ICommandFactory<PickUpItemCommand> PickUpItemCommandFactory { get; set; }
        public ICommandFactory<ChangeMapCommand> ChangeMapCommandFactory { get; set; }
        public ICommandFactory<ApplyItemCommand> ApplyItemCommandFactory { get; set; }
        public ICommandFactory<ApplyShieldCommand> ApplyShieldCommandFactory { get; set; }
        public ICommandFactory<ApplyHealingBotsCommand> ApplyHealingBotsCommandFactory { get; set; }
        public ICommandFactory<EnchantItemCommand> EnchantItemCommandFactory { get; set; }
        public ICommandFactory<WaitCommand> WaitCommandFactory { get; set; }
        public ICommandFactory<ApplyMachineCommand> ApplyMachineCommandFactory { get; set; }
        public ICommandFactory<IdentifyItemCommand> IdentifyItemCommandFactory { get; set; }
        public ICommandFactory<UndoCommand> UndoCommandFactory { get; set; }
        public ICommandFactory<ApplyForcePushCommand> ApplyForcePushCommandFactory { get; set; }
        public ICommandFactory<PlayerRangeAttackCommand> PlayerRangeAttackCommandFactory { get; set; }
        public ICommandFactory<ExplodeTileCommand> ExplodeTileCommandFactory { get; set; }
        public ICommandFactory<SwapPositionCommand> SwapPositionCommandFactory { get; set; }

        // Note commands are not persisted between turns or on load game
        public List<BaseGameActionCommand> Commands { get; set; } = new List<BaseGameActionCommand>();

        public bool ClearCommandsOnNextAdd { get; set; }

        public T CreateCommand<T>(IGameWorld gameWorld) where T : BaseGameActionCommand
        {
            if (ClearCommandsOnNextAdd)
            {
                Commands.Clear();
                ClearCommandsOnNextAdd = false;
            }

            var command = CreateCommand<T>(gameWorld, _nextId);
            _nextId++;
            
            return command;
        }
        
        public T CreateCommand<T>(IGameWorld gameWorld, uint id) where T : BaseGameActionCommand
        {
            var factoryName = $"{typeof(T).Name}Factory";
            
            // use reflection to look up property called factoryName
            var factory = (ICommandFactory<T>)GetType().GetProperty(factoryName)?.GetValue(this);
            
            if (factory == null)
            {
                throw new Exception($"Factory not found for {typeof(T).Name}. " +
                    $"Ensure it is added to the installer, then add a property called {factoryName} to this class.");
            }
            
            var command = factory.Create(gameWorld);

            command.Id = id;

            Commands.Add(command);

            return command;
        }

        public BaseGameActionCommand GetCommand(uint commandId)
        {
            return Commands.First(c => c.Id == commandId);
        }

        public T GetLastCommand<T>() where T : BaseGameActionCommand
        {
            for (var i = Commands.Count - 1; i >= 0; i--)
                if (Commands[i] is T)
                    return Commands[i] as T;

            return null;
        }

        public List<T> GetLastCommands<T>() where T : BaseGameActionCommand
        {
            return Commands.OfType<T>().ToList();
        }

        public void Initialise()
        {
            Commands.Clear();

            _nextId = 1;
        }
    }
}