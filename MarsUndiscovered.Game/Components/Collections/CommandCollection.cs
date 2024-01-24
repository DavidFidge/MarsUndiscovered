using System.Collections;
using System.Reflection;

using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Services;
using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components
{
    public class CommandCollection : IMementoState<CommandCollectionSaveData>, ICommandCollection
    {
        private uint _nextId = 1;

        public List<LineAttackCommand> LineAttackCommands { get; private set; } = new();
        public ICommandFactory<LineAttackCommand> LineAttackCommandFactory { get; set; }
        
        public List<MeleeAttackCommand> MeleeAttackCommands { get; private set; } = new();
        public ICommandFactory<MeleeAttackCommand> MeleeAttackCommandFactory { get; set; }
        
        public List<LightningAttackCommand> LightningAttackCommands { get; private set; } = new();
        public ICommandFactory<LightningAttackCommand> LightningAttackCommandFactory { get; set; }

        public List<WalkCommand> WalkCommands { get; private set; } = new();
        public ICommandFactory<WalkCommand> WalkCommandFactory { get; set; }

        public List<MoveCommand> MoveCommands { get; private set; } = new();
        public ICommandFactory<MoveCommand> MoveCommandFactory { get; set; }
        
        public List<DeathCommand> DeathCommands { get; private set; } = new();
        public ICommandFactory<DeathCommand> DeathCommandFactory { get; set; }

        public List<EquipItemCommand> EquipItemCommands { get; private set; } = new();
        public ICommandFactory<EquipItemCommand> EquipItemCommandFactory { get; set; }

        public List<UnequipItemCommand> UnequipItemCommands { get; private set; } = new();
        public ICommandFactory<UnequipItemCommand> UnequipItemCommandFactory { get; set; }

        public List<DropItemCommand> DropItemCommands { get; private set; } = new();
        public ICommandFactory<DropItemCommand> DropItemCommandFactory { get; set; }

        public List<PickUpItemCommand> PickUpItemCommands { get; private set; } = new();
        public ICommandFactory<PickUpItemCommand> PickUpItemCommandFactory { get; set; }

        public List<ChangeMapCommand> ChangeMapCommands { get; private set; } = new();
        public ICommandFactory<ChangeMapCommand> ChangeMapCommandFactory { get; set; }
        
        public List<ApplyItemCommand> ApplyItemCommands { get; private set; } = new();
        public ICommandFactory<ApplyItemCommand> ApplyItemCommandFactory { get; set; }

        public List<ApplyShieldCommand> ApplyShieldCommands { get; private set; } = new();
        public ICommandFactory<ApplyShieldCommand> ApplyShieldCommandFactory { get; set; }
        
        public List<ApplyHealingBotsCommand> ApplyHealingBotsCommands { get; private set; } = new();
        public ICommandFactory<ApplyHealingBotsCommand> ApplyHealingBotsCommandFactory { get; set; }
        
        public List<EnchantItemCommand> EnchantItemCommands { get; private set; } = new();
        public ICommandFactory<EnchantItemCommand> EnchantItemCommandFactory { get; set; }

        public List<WaitCommand> WaitCommands { get; private set; } = new();
        public ICommandFactory<WaitCommand> WaitCommandFactory { get; set; }

        public List<ApplyMachineCommand> ApplyMachineCommands { get; private set; } = new();
        public ICommandFactory<ApplyMachineCommand> ApplyMachineCommandFactory { get; set; }

        public List<IdentifyItemCommand> IdentifyItemCommands { get; private set; } = new();
        public ICommandFactory<IdentifyItemCommand> IdentifyItemCommandFactory { get; set; }
        
        public List<UndoCommand> UndoCommands { get; private set; } = new();
        public ICommandFactory<UndoCommand> UndoCommandFactory { get; set; }
        
        public List<ApplyForcePushCommand> ApplyForcePushCommand { get; private set; } = new();
        public ICommandFactory<ApplyForcePushCommand> ApplyForcePushCommandFactory { get; set; }
        
        private List<uint> _replayCommandIds = new();
        private Dictionary<uint, BaseGameActionCommand> _commandsById = new();
        private Dictionary<Type, PropertyInfo> _commandListProperties;
        
        public CommandCollection()
        {
            _commandListProperties = GetCommandListProperties();
        }
        
        public IMemento<CommandCollectionSaveData> GetSaveState()
        {
            return new Memento<CommandCollectionSaveData>
            {
                State = new CommandCollectionSaveData
                {
                    NextId = _nextId,
                    ReplayCommandIds = _replayCommandIds.ToList()
                }
            };
        }

        public void SetLoadState(IMemento<CommandCollectionSaveData> memento)
        {
            _nextId = memento.State.NextId;
            _replayCommandIds = memento.State.ReplayCommandIds.ToList();
        }

        public void SaveState(ISaveGameService saveGameService, IGameWorld gameWorld)
        {
            var saveState = GetSaveState();
            saveGameService.SaveToStore(saveState);
            
            // call SaveCommandList for each of properties using reflection
            foreach (var commandListProperty in _commandListProperties)
            {
                var method = this.GetType().GetMethod("SaveCommandList");

                var commandTypeSaveData = commandListProperty.Key.BaseType.GetGenericArguments().First();
                var commandList = commandListProperty.Value.GetValue(this);
                
                var genericMethod = method.MakeGenericMethod(commandListProperty.Key, commandTypeSaveData);
                
                genericMethod.Invoke(this, new object[] { commandList, saveGameService });
            }
        }

        public void LoadState(ISaveGameService saveGameService, IGameWorld gameWorld)
        {
            var memento = saveGameService.GetFromStore<CommandCollectionSaveData>();
            
            _nextId = memento.State.NextId;
            _replayCommandIds = memento.State.ReplayCommandIds.ToList();
            
            foreach (var commandListProperty in _commandListProperties)
            {
                // Call LoadCommandsList for each property using reflection
                var method = this.GetType().GetMethod("LoadCommandList");

                var commandTypeSaveData = commandListProperty.Key.BaseType.GetGenericArguments().First();
                
                var commandList = commandListProperty.Value.GetValue(this);

                var genericMethod = method.MakeGenericMethod(commandListProperty.Key, commandTypeSaveData);
                
                genericMethod.Invoke(this, new object[] { commandList, saveGameService, gameWorld });
            }
        }

        private Dictionary<Type, PropertyInfo> GetCommandListProperties()
        {
            var commandListProperties = this
                .GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType.IsAssignableTo(typeof(IList)))
                .Where(p => p.PropertyType.GetGenericArguments().Any())
                .Where(p => p.PropertyType.GetGenericArguments().First().IsAssignableTo(typeof(BaseGameActionCommand)))
                .ToDictionary(k => k.PropertyType.GetGenericArguments().First(), v => v);
            
            return commandListProperties;
        }

        // Called via reflection
        public void SaveCommandList<T, TSaveData>(List<T> commands, ISaveGameService saveGameService)
            where T : BaseMarsGameActionCommand<TSaveData>
            where TSaveData : BaseCommandSaveData, new()
        {
            var mementos = new List<IMemento<TSaveData>>();
            
            foreach (var command in commands)
            {
                var saveState = command.GetSaveState();
                
                mementos.Add(saveState);
            }

            saveGameService.SaveListToStore(mementos);
        }

        // Called via reflection
        public void LoadCommandList<T, TSaveData>(List<T> commands, ISaveGameService saveGameService, IGameWorld gameWorld)
            where T : BaseMarsGameActionCommand<TSaveData>
            where TSaveData : BaseCommandSaveData, new()
        {
            var mementos = saveGameService.GetListFromStore<TSaveData>();
            
            foreach (var memento in mementos)
            {
                var command = CreateCommand<T>(gameWorld, memento.State.Id);
                
                command.SetLoadState(memento);

                commands.Add(command);
            }
        }
        
        public T CreateCommand<T>(IGameWorld gameWorld) where T : BaseGameActionCommand
        {
            var command = CreateCommand<T>(gameWorld, _nextId);
            _nextId++;
            
            return command;
        }
        
        public T CreateCommand<T>(IGameWorld gameWorld, uint id) where T : BaseGameActionCommand
        {
            var factoryName = $"{typeof(T).Name}Factory";
            
            // use reflection to look up property called factoryName
            var factory = (ICommandFactory<T>)this.GetType().GetProperty(factoryName)?.GetValue(this);
            
            if (factory == null)
            {
                throw new Exception($"Factory not found for {typeof(T).Name}. " +
                    $"Ensure it is added to the installer, then add a property called {factoryName} to this class.");
            }
            
            var command = factory.Create(gameWorld);

            command.Id = id;
            _commandsById.Add(id, command);
            
            var commandListProperty = _commandListProperties[typeof(T)];
            
            var commandList = (List<T>)commandListProperty.GetValue(this);

            commandList.Add(command);

            return command;
        }
        
        // Usually commands are created by the factories in this class but when replaying
        // the command is already created. This method is used to add it to the collection
        public void ReprocessReplayCommand<T>(T command) where T : BaseGameActionCommand
        {
            if (_nextId != command.Id)
            {
                throw new Exception(
                    $"Expected _nextId {_nextId} to be equal to the command being replayed: {command.Id}");
            }
            
            _commandsById.Add(command.Id, command);
            
            // command.GetType is used here instead of typeof(T) because
            // the parameter passed in is a BaseGameActionCommand rather than
            // the actual concrete type.
            var commandListProperty = _commandListProperties[command.GetType()];
            
            var commandList = (IList)commandListProperty.GetValue(this);

            commandList.Add(command);
            _nextId++;
        }

        public void AddReplayCommand(BaseGameActionCommand command)
        {
            _replayCommandIds.Add(command.Id);
        }

        public BaseGameActionCommand[] GetReplayCommands()
        {
            return _replayCommandIds
                .Select(r => _commandsById[r])
                .OrderBy(c => c.TurnDetails.SequenceNumber)
                .ToArray();
        }

        public T GetLastCommand<T>()
        {
            var commandListProperty = _commandListProperties[typeof(T)];
            
            var commandList = (IList)commandListProperty.GetValue(this);
            
            return (T)commandList[^1];
        }

        public List<T> GetCommands<T>()
        {
            var commandListProperty = _commandListProperties[typeof(T)];
            
            var commandList = commandListProperty.GetValue(this);

            return (List<T>)commandList;
        }

        public void Initialise()
        {
            foreach (var commandListProperty in _commandListProperties.Values)
            {
                var commandList = (IList)commandListProperty.GetValue(this);
                commandList.Clear();
            }

            _nextId = 1;
            _replayCommandIds.Clear();
            _commandsById.Clear();
        }

        public BaseGameActionCommand GetCommand(uint dataCommandId)
        {
            return _commandsById[dataCommandId];
        }
    }
}