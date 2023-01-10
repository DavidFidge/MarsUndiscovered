using System.Collections;
using System.Reflection;

using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;

using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Components
{
    public class CommandCollection : IEnumerable<BaseGameActionCommand>, ISaveable
    {
        private readonly List<PropertyInfo> _commandCollectionPropertyInfos;

        public CommandCollection(ICommandFactory commandFactory, IGameWorld gameWorld)
        {
            AttackCommands = new AttackCommandCollection(commandFactory, gameWorld);
            LightningAttackCommands = new LightningAttackCommandCollection(commandFactory, gameWorld);
            WalkCommands = new WalkCommandCollection(commandFactory, gameWorld);
            MoveCommands = new MoveCommandCollection(commandFactory, gameWorld);
            DeathCommands = new DeathCommandCollection(commandFactory, gameWorld); 
            EquipItemCommands = new EquipItemCommandCollection(commandFactory, gameWorld);
            UnequipItemCommands = new UnequipItemCommandCollection(commandFactory, gameWorld);
            DropItemCommands = new DropItemCommandCollection(commandFactory, gameWorld);
            PickUpItemCommands = new PickUpItemCommandCollection(commandFactory, gameWorld);
            ChangeMapCommands = new ChangeMapCommandCollection(commandFactory, gameWorld);

            _commandCollectionPropertyInfos = GetType()
                .GetProperties()
                .Where(p => p.PropertyType.BaseType != null)
                .Where(p => p.PropertyType.BaseType.GenericTypeArguments.Any())
                .Where(
                    p => typeof(BaseGameActionCommand).IsAssignableFrom(p.PropertyType.BaseType.GenericTypeArguments[0])
                )
                .Where(p => typeof(IList).IsAssignableFrom(p.PropertyType))
                .ToList();
        }

        public AttackCommandCollection AttackCommands { get; set; }
        public LightningAttackCommandCollection LightningAttackCommands { get; set; }
        public WalkCommandCollection WalkCommands { get; set; }
        public MoveCommandCollection MoveCommands { get; set; }
        public DeathCommandCollection DeathCommands { get; set; }
        public EquipItemCommandCollection EquipItemCommands { get; set; }
        public UnequipItemCommandCollection UnequipItemCommands { get; set; }
        public DropItemCommandCollection DropItemCommands { get; set; }
        public PickUpItemCommandCollection PickUpItemCommands { get; set; }
        public ChangeMapCommandCollection ChangeMapCommands { get; set; }

        public IEnumerator<BaseGameActionCommand> GetEnumerator()
        {
            foreach (var commandCollectionProperty in _commandCollectionPropertyInfos)
            {
                var commandCollection = (IList)commandCollectionProperty.GetValue(this);

                foreach (var command in commandCollection)
                {
                    yield return (BaseGameActionCommand)command;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void SaveState(ISaveGameService saveGameService)
        {
            SaveLoad(saveGameService, (s, sgs) => s.SaveState(sgs));
        }

        public void LoadState(ISaveGameService saveGameService)
        {
            SaveLoad(saveGameService, (s, sgs) => s.LoadState(sgs));
        }

        private void SaveLoad(ISaveGameService saveGameService, Action<ISaveable, ISaveGameService> action)
        {
            foreach (var property in _commandCollectionPropertyInfos)
            {
                var saveable = (ISaveable)property.GetValue(this);
                action(saveable, saveGameService);
            }
        }

        public void AddCommand(BaseGameActionCommand command)
        {
            var commandType = command.GetType();

            var propertyInfo = _commandCollectionPropertyInfos.First(p => p.PropertyType.BaseType.GenericTypeArguments[0].IsAssignableFrom(commandType));

            var commandCollection = (IList)propertyInfo.GetValue(this);

            commandCollection.Add(command);
        }
    }
}