using MarsUndiscovered.Game.Components.Factories;

namespace MarsUndiscovered.Game.Components.Maps
{
    public interface IMachineGenerator
    {
        void SpawnMachine(SpawnMachineParams spawnMachineParams, IGameObjectFactory gameObjectFactory, MapCollection maps, MachineCollection machineCollection);
    }
}