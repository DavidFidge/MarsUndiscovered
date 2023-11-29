using System.Collections;
using System.Reflection;

using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components
{
    public class CommandCollection : ISaveable
    {
        public List<LineAttackCommand> LineAttackCommands { get; set; }
        public List<MeleeAttackCommand> MeleeAttackCommands { get; set; }
        public List<LightningAttackCommand> LightningAttackCommands { get; set; }
        public List<WalkCommand> WalkCommands { get; set; }
        public List<MoveCommand> MoveCommands { get; set; }
        public List<DeathCommand> DeathCommands { get; set; }
        public List<EquipItemCommand> EquipItemCommands { get; set; }
        public List<UnequipItemCommand> UnequipItemCommands { get; set; }
        public List<DropItemCommand> DropItemCommands { get; set; }
        public List<PickUpItemCommand> PickUpItemCommands { get; set; }
        public List<ChangeMapCommand> ChangeMapCommands { get; set; }
        public List<ApplyItemCommand> ApplyItemCommands { get; set; }
        public List<ApplyShieldCommand> ApplyShieldCommands { get; set; }
        public List<ApplyHealingBotsCommand> ApplyHealingBotsCommands { get; set; }
        public List<EnchantItemCommand> EnchantItemCommands { get; set; }
        public List<WaitCommand> WaitCommands { get; set; }
        public List<ApplyMachineCommand> ApplyMachineCommands { get; set; }
        public List<IdentifyItemCommand> IdentifyItemCommands { get; set; }
        public List<UndoCommand> UndoCommands { get; set; }
        
        public CommandCollection(ICommandFactory commandFactory, IGameWorld gameWorld)
        {
        }
        
        public void SaveState(ISaveGameService saveGameService, IGameWorld gameWorld)
        {
            var commandLists = GetType()
                .GetProperties()
                .Where(p => p.PropertyType.GenericTypeArguments.Any())
                .Where(
                    p => typeof(IBaseMarsGameActionCommand).IsAssignableFrom(p.PropertyType.GenericTypeArguments[0])
                )
                .Where(p => typeof(IList).IsAssignableFrom(p.PropertyType))
                .ToDictionary(p => p.PropertyType.GenericTypeArguments[0], p => p.GetValue(this));
            
            foreach (var commandList in commandLists)
            {
                // using commandList.Key, get the base class type's first generic argument type
                var saveDataType = commandList.Key.BaseType.GenericTypeArguments[0];
                
                var mementos = ((IList)commandList.Value)
                    .Cast<IBaseMarsGameActionCommand>()
                    .Select(c => c.GetSaveStateAsObject())
                    .ToList();
                
                // Call savegameService.SaveListToStore using gameObjectSaveData's type
                saveGameService.GetType().GetMethod(nameof(saveGameService.SaveListToStore))
                    .MakeGenericMethod(saveDataType)
                    .Invoke(saveGameService, new [] { mementos });
            }
        }

        public void LoadState(ISaveGameService saveGameService, IGameWorld gameWorld)
        {
            //SaveLoad(saveGameService, (s, sgs) => s.LoadState(sgs, gameWorld));
        }
    }
}