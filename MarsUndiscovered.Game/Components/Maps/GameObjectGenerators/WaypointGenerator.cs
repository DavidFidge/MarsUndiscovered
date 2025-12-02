using System.Reflection.PortableExecutable;

using GoRogue.MapGeneration;

using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.Maps.MapPointChoiceRules;
using MarsUndiscovered.Game.Extensions;
using MarsUndiscovered.Game.ViewMessages;
using MarsUndiscovered.Interfaces;

using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Components.Maps
{
    public class WaypointGenerator : BaseGameObjectGenerator, IWaypointGenerator
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
            spawnMachineParams.MapPointChoiceRules.Add(new NonBlockingRule());
            spawnMachineParams.AssignMap(map);

            // Machines are blocking so only spawn a machine if it can be moved past
            var position = GetPosition(spawnMachineParams, map);
                
            if (position == Point.None)
                return;
            
            machine.PositionedAt(position)
                .AddToMap(map);

            machineCollection.Add(machine.ID, machine);
            
            Mediator.Publish(new MapTileChangedNotification(machine.Position));

            spawnMachineParams.Result = machine;
        }

        public void SpawnWaypoint(SpawnWaypointParams spawnWaypointParams, IGameWorld gameWorld)
        {
            spawnWaypointParams.Result = null;

            if (spawnWaypointParams.Name == null)
                return;

            var map = gameWorld.Maps.Single(m => m.Id == spawnWaypointParams.MapId);

            var waypoint = gameWorld.GameObjectFactory
                .CreateGameObject<Waypoint>()
                .WithName(spawnWaypointParams.Name);

            spawnWaypointParams.AssignMap(map);

            var position = GetPosition(spawnWaypointParams, map);

            // This is highly unlikely to happen if everything is working because waypoints
            // are placed before any other object types and it should always be on a floor
            if (position == Point.None)
                throw new RegenerateMapException();

            waypoint.PositionedAt(position)
                .AddToMap(map);

            gameWorld.Waypoints.Add(waypoint.ID, waypoint);

            spawnWaypointParams.Result = waypoint;
        }
    }
}
