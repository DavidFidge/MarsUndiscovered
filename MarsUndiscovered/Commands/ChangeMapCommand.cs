using System;
using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using GoRogue.GameFramework;
using GoRogue.Pathing;

using MarsUndiscovered.Components;
using MarsUndiscovered.Interfaces;
using MarsUndiscovered.Messages;

using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

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

        public override IMemento<ChangeMapSaveData> GetSaveState(IMapper mapper)
        {
            return Memento<ChangeMapSaveData>.CreateWithAutoMapper(this, mapper);
        }

        public override void SetLoadState(IMemento<ChangeMapSaveData> memento, IMapper mapper)
        {
            base.SetLoadState(memento, mapper);

            Memento<ChangeMapSaveData>.SetWithAutoMapper(this, memento, mapper);
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
                Mediator.Publish(new MapTileChangedNotification(_oldPosition));
            }
        }
    }
}
