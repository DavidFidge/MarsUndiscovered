using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Components.Maps;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components
{
    public class Spawner : BaseComponent, ISpawner
    {
        public IMonsterGenerator MonsterGenerator { get; set; }
        public IItemGenerator ItemGenerator { get; set; }
        public IMapExitGenerator MapExitGenerator { get; set; }
        public IMachineGenerator MachineGenerator { get; set; }
        public IEnvironmentalEffectGenerator EnvironmentalEffectGenerator { get; set; }
        public IFeatureGenerator FeatureGenerator { get; set; }
        private GameWorld _gameWorld { get; set; }


        public void Initialise(GameWorld gameWorld)
        {
            _gameWorld = gameWorld;
        }

        public void SpawnMonster(SpawnMonsterParams spawnMonsterParams)
        {
            MonsterGenerator.SpawnMonster(spawnMonsterParams, _gameWorld);
        }

        public void SpawnItem(SpawnItemParams spawnItemParams)
        {
            if (spawnItemParams.IntoPlayerInventory)
                spawnItemParams.Inventory = _gameWorld.Inventory;

            ItemGenerator.SpawnItem(spawnItemParams, _gameWorld.GameObjectFactory, _gameWorld.Maps, _gameWorld.Items);
        }

        public void SpawnMapExit(SpawnMapExitParams spawnMapExitParams)
        {
            MapExitGenerator.SpawnMapExit(spawnMapExitParams, _gameWorld);
        }

        public void SpawnMachine(SpawnMachineParams spawnMachineParams)
        {
            MachineGenerator.SpawnMachine(spawnMachineParams, _gameWorld.GameObjectFactory, _gameWorld.Maps, _gameWorld.Machines);
        }

        public void SpawnEnvironmentalEffect(SpawnEnvironmentalEffectParams spawnEnvironmentalEffectParams)
        {
            EnvironmentalEffectGenerator.SpawnEnvironmentalEffect(spawnEnvironmentalEffectParams, _gameWorld.GameObjectFactory, _gameWorld.Maps, _gameWorld.EnvironmentalEffects);
        }

        public void SpawnFeature(SpawnFeatureParams spawnFeatureParams)
        {
            FeatureGenerator.SpawnFeature(spawnFeatureParams, _gameWorld.GameObjectFactory, _gameWorld.Maps, _gameWorld.Features);
        }
    }
}
