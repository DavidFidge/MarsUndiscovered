using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;
using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Interfaces;
using Exception = System.Exception;

namespace MarsUndiscovered.Game.Components.Factories
{
    public class CommandFactory : ICommandFactory, IMementoState<CommandFactorySaveData>
    {
        private uint _nextId = 1;
        
        // The factories are injected by windsor then reflection is used
        // to look up the factory to use in CreateCommand
        public ICommandFactory<MoveCommand> MoveCommandFactory { get; set; }
        public ICommandFactory<WalkCommand> WalkCommandFactory { get; set; }
        public ICommandFactory<MeleeAttackCommand> MeleeAttackCommandFactory { get; set; }
        public ICommandFactory<LineAttackCommand> LineAttackCommandFactory { get; set; }
        public ICommandFactory<LightningAttackCommand> LightningAttackCommandFactory { get; set; }
        public ICommandFactory<DeathCommand> DeathCommandFactory { get; set; }
        public ICommandFactory<PickUpItemCommand> PickUpItemCommandFactory { get; set; }
        public ICommandFactory<EquipItemCommand> EquipItemCommandFactory { get; set; }
        public ICommandFactory<UnequipItemCommand> UnequipItemCommandFactory { get; set; }
        public ICommandFactory<DropItemCommand> DropItemCommandFactory { get; set; }
        public ICommandFactory<ChangeMapCommand> ChangeMapCommandFactory { get; set; }
        public ICommandFactory<ApplyItemCommand> ApplyItemCommandFactory { get; set; }
        public ICommandFactory<ApplyShieldCommand> ApplyShieldCommandFactory { get; set; }
        public ICommandFactory<ApplyHealingBotsCommand> ApplyHealingBotsCommandFactory { get; set; }
        public ICommandFactory<EnchantItemCommand> EnchantItemCommandFactory { get; set; }
        public ICommandFactory<WaitCommand> WaitCommandFactory { get; set; }
        public ICommandFactory<ApplyMachineCommand> ApplyMachineCommandFactory { get; set; }
        public ICommandFactory<IdentifyItemCommand> IdentifyItemCommandFactory { get; set; }
        public ICommandFactory<UndoCommand> UndoCommandFactory { get; set; }

        // This is not persisted. It can be used during normal game/replay processing but not used
        // during save/load.
        public List<BaseGameActionCommand> CreatedCommands { get; private set; }
        
        public T CreateCommand<T>(IGameWorld gameWorld) where T : BaseGameActionCommand
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

            command.Id = _nextId;
            _nextId++;
            CreatedCommands.Add(command);

            return command;
        }

        public IMemento<CommandFactorySaveData> GetSaveState()
        {
            return new Memento<CommandFactorySaveData>()
            {
                State = new CommandFactorySaveData()
                {
                    NextId = _nextId
                }
            };
        }

        public void SetLoadState(IMemento<CommandFactorySaveData> memento)
        {
            this._nextId = memento.State.NextId;
        }
    }
}
