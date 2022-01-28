
using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using GoRogue.GameFramework;

using MarsUndiscovered.Components;
using MarsUndiscovered.Interfaces;
using MarsUndiscovered.Messages;

using SadRogue.Primitives;

namespace MarsUndiscovered.Commands
{
    public class ChangeMapCommand : BaseMarsGameActionCommand<ChangeMapSaveData>
    {
        private MarsMap _oldMap;
        private Point _oldPosition;
        public IGameObject GameObject { get; private set; }
        public MapExit MapExit { get; set; }

        public ChangeMapCommand(IGameWorld gameWorld) : base(gameWorld)
        {
        }

        public void Initialise(IGameObject gameObject, MapExit mapExit)
        {
            GameObject = gameObject;
            MapExit = mapExit;
        }

        public override IMemento<ChangeMapSaveData> GetSaveState()
        {
            var memento = new Memento<ChangeMapSaveData>();
            base.PopulateSaveState(memento.State);
            memento.State.GameObjectId = GameObject.ID;
            memento.State.MapExitId = MapExit.ID;

            return memento;
        }

        public override void SetLoadState(IMemento<ChangeMapSaveData> memento)
        {
            base.PopulateLoadState(memento.State);

            GameObject = GameWorld.GameObjects[memento.State.GameObjectId];
            MapExit = (MapExit)GameWorld.GameObjects[memento.State.MapExitId];
        }

        protected override CommandResult ExecuteInternal()
        {
            _oldMap = (MarsMap)GameObject.CurrentMap;
            _oldPosition = GameObject.Position;

            _oldMap.RemoveEntity(GameObject);

            var newMap = (MarsMap)MapExit.Destination.CurrentMap;

            GameObject.Position = newMap.FindClosestFreeFloor(MapExit.Destination.LandingPosition);
            newMap.AddEntity(GameObject);

            if (GameObject is Player)
            {
                GameWorld.ChangeMap(newMap);

                var exitText = MapExit.Direction == Direction.Down ? "descend" : "ascend";
                return Result(CommandResult.Success(this, $"You {exitText}"));
            }
            else
            {
                GameWorld.RebuildGoalMaps();

                Mediator.Publish(new MapTileChangedNotification(GameObject.Position));
                return Result(CommandResult.Success(this));
            }
        }

        protected override void UndoInternal()
        {
            GameObject.CurrentMap.RemoveEntity(GameObject);
            GameObject.Position = _oldPosition;
            _oldMap.AddEntity(GameObject);

            if (GameObject is Player)
            {
                GameWorld.ChangeMap(_oldMap);
            }
            else
            {
                GameWorld.RebuildGoalMaps();

                Mediator.Publish(new MapTileChangedNotification(_oldPosition));
            }
        }
    }
}
