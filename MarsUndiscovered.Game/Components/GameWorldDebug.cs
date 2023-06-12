using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Interfaces;
using MarsUndiscovered.Game.Components.Maps;

namespace MarsUndiscovered.Game.Components
{
    public class GameWorldDebug : BaseComponent, IGameWorldDebug
    {
        public IMonsterGenerator MonsterGenerator { get; set; }
        public IItemGenerator ItemGenerator { get; set; }
        public IMapExitGenerator MapExitGenerator { get; set; }
        private GameWorld _gameWorld { get; set; }


        public void Initialise(GameWorld gameWorld)
        {
            _gameWorld = gameWorld;
        }

        public void SpawnMonster(SpawnMonsterParams spawnMonsterParams)
        {
            MonsterGenerator.SpawnMonster(spawnMonsterParams, _gameWorld.GameObjectFactory, _gameWorld.Maps, _gameWorld.Monsters);
        }

        public void SpawnItem(SpawnItemParams spawnItemParams)
        {
            if (spawnItemParams.IntoPlayerInventory)
                spawnItemParams.Inventory = _gameWorld.Inventory;

            ItemGenerator.SpawnItem(spawnItemParams, _gameWorld.GameObjectFactory, _gameWorld.Maps, _gameWorld.Items);
        }

        public void SpawnMapExit(SpawnMapExitParams spawnMapExitParams)
        {
            MapExitGenerator.SpawnMapExit(spawnMapExitParams, _gameWorld.GameObjectFactory, _gameWorld.Maps, _gameWorld.MapExits);
        }
    }
}
