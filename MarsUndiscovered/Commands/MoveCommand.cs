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
    public class MoveCommand : BaseMarsGameActionCommand<MoveCommandSaveData>
    {
        public IGameObject GameObject { get; private set; }
        public Tuple<Point, Point> FromTo { get; set; }

        public MoveCommand(IGameWorld gameWorld) : base(gameWorld)
        {
        }

        public void Initialise(IGameObject gameObject, Tuple<Point, Point> fromTo)
        {
            GameObject = gameObject;
            FromTo = fromTo;
        }

        public override IMemento<MoveCommandSaveData> GetSaveState(IMapper mapper)
        {
            return Memento<MoveCommandSaveData>.CreateWithAutoMapper(this, mapper);
        }

        public override void SetLoadState(IMemento<MoveCommandSaveData> memento, IMapper mapper)
        {
            base.SetLoadState(memento, mapper);

            Memento<MoveCommandSaveData>.SetWithAutoMapper(this, memento, mapper);
            GameObject = GameWorld.GameObjects[memento.State.GameObjectId];
        }

        protected override CommandResult ExecuteInternal()
        {
            var subsequentCommands = new List<BaseGameActionCommand>();

            GameObject.Position = FromTo.Item2;

            if (GameObject is Player)
            {
                var item = GameObject.CurrentMap.GetObjectAt<Item>(GameObject.Position);

                if (item != null)
                {
                    var pickUpItemCommand = CommandFactory.CreatePickUpItemCommand(GameWorld);
                    pickUpItemCommand.Initialise(item, GameObject);
                    subsequentCommands.Add(pickUpItemCommand);
                }
            }

            Mediator.Publish(new MapTileChangedNotification(FromTo.Item1));
            Mediator.Publish(new MapTileChangedNotification(FromTo.Item2));

            return Result(CommandResult.Success(this, subsequentCommands));
        }

        protected override void UndoInternal()
        {
            GameObject.Position = FromTo.Item1;
        }
    }
}
