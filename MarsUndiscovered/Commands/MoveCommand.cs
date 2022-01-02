using System;
using System.Collections.Generic;
using System.Text;

using AutoMapper;

using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using GoRogue.GameFramework;

using MarsUndiscovered.Messages;

using Point = SadRogue.Primitives.Point;

namespace MarsUndiscovered.Commands
{
    public class MoveCommand : BaseMarsGameActionCommand<MoveCommandSaveData>
    {
        public IGameObject GameObject { get; private set; }
        public Tuple<Point, Point> FromTo { get; set; }

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
            GameObject.Position = FromTo.Item2;

            Mediator.Send(new MapTileChangedRequest(FromTo.Item1));
            Mediator.Send(new MapTileChangedRequest(FromTo.Item2));

            return Result(CommandResult.Success());
        }

        protected override void UndoInternal()
        {
            GameObject.Position = FromTo.Item1;
        }
    }
}
