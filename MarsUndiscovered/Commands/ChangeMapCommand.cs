using System;
using System.Collections.Generic;

using AutoMapper;

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
            var subsequentCommands = new List<BaseGameActionCommand>();

            _oldMap = (MarsMap)GameObject.CurrentMap;
            _oldPosition = GameObject.Position;

            _oldMap.RemoveEntity(GameObject);

            // TODO - need to check if the position is taken
            GameObject.Position = MapExit.Destination.LandingPosition;
            MapExit.Destination.CurrentMap.AddEntity(GameObject);

            if (GameObject is Player)
            {
                GameWorld.ChangeMap((MarsMap)MapExit.Destination.CurrentMap);
            }
            else
            {
                GameWorld.RebuildGoalMaps();

                Mediator.Publish(new MapTileChangedNotification(GameObject.Position));
            }

            return Result(CommandResult.Success(this, subsequentCommands));
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
