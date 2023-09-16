using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.Maps.MapPointChoiceRules;
using MarsUndiscovered.Game.Extensions;
using MarsUndiscovered.Game.ViewMessages;

namespace MarsUndiscovered.Game.Components.Maps
{
    public class MachineGenerator : BaseGameObjectGenerator, IMachineGenerator
    {
        public void SpawnMachine(SpawnMachineParams spawnMachineParams, IGameObjectFactory gameObjectFactory, MapCollection maps, MachineCollection machineCollection)
        {
            spawnMachineParams.Result = null;
            
            if (spawnMachineParams.MachineType == null)
                return;

            var machine = gameObjectFactory
                .CreateGameObject<Machine>()
                .WithMachineType(spawnMachineParams.MachineType);

            var map = maps.Single(m => m.Id == spawnMachineParams.MapId);
            spawnMachineParams.MapPointChoiceRules.Add(new EmptyFloorRule());
            spawnMachineParams.AssignMap(map);

            var position = GetPosition(spawnMachineParams, map);
                
            machine.PositionedAt(position)
                .AddToMap(map);

            machineCollection.Add(machine.ID, machine);
            
            Mediator.Publish(new MapTileChangedNotification(machine.Position));

            spawnMachineParams.Result = machine;
        }
    }
}
