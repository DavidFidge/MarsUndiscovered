namespace MarsUndiscovered.Game.Components
{
    public class SpawnMachineParams : BaseSpawnGameObjectParams
    {
        public MachineType MachineType { get; set; }
        public Machine Result { get; set; }
    }

    public static class SpawnMachineParamsFluentExtensions
    {
        public static SpawnMachineParams WithMachineType(this SpawnMachineParams spawnMachineParams, MachineType machineType)
        {
            spawnMachineParams.MachineType = machineType;
            return spawnMachineParams;
        }
    }
}