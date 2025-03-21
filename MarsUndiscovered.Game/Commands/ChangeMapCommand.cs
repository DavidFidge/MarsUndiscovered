using FrigidRogue.MonoGame.Core.Components;
using GoRogue.GameFramework;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.ViewMessages;
using MarsUndiscovered.Interfaces;
using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Commands
{
    public class ChangeMapCommand : BaseMarsGameActionCommand<ChangeMapSaveData>
    {
        private MarsMap _oldMap => GameWorld.Maps.Single(m => m.Id == _data.OldMapId);
        public IGameObject GameObject => GameWorld.GameObjects[_data.GameObjectId];
        public MapExit MapExit => GameWorld.GameObjects[_data.MapExitId] as MapExit;

        public ChangeMapCommand(IGameWorld gameWorld) : base(gameWorld)
        {
        }

        public void Initialise(IGameObject gameObject, MapExit mapExit)
        {
            _data.GameObjectId = gameObject.ID;
            _data.MapExitId = mapExit.ID;
        }
        
        protected override CommandResult ExecuteInternal()
        {
            _data.OldMapId = ((MarsMap)GameObject.CurrentMap).Id;
            _data.OldPosition = GameObject.Position;

            _oldMap.RemoveEntity(GameObject);

            var newMap = (MarsMap)MapExit.Destination.CurrentMap;

            GameObject.Position = newMap.FindClosestFreeFloor(MapExit.Destination.LandingPosition);

            if (GameObject.Position == Point.None)
            {
                // TODO: Should destroy a monster to place the player if no room
                throw new Exception("No room to place player");
            }
            
            newMap.AddEntity(GameObject);

            if (GameObject is Player)
            {
                GameWorld.ChangeMap(newMap);
                
                return Result(CommandResult.Success(this, MapExit.ExitText));
            }

            Mediator.Publish(new MapTileChangedNotification(GameObject.Position));
            return Result(CommandResult.Success(this));
        }
    }
}
