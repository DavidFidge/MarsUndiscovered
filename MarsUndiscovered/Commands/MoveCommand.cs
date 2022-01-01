using System;
using System.Collections.Generic;
using System.Text;

using AutoMapper;

using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using GoRogue.GameFramework;

using MarsUndiscovered.Interfaces;
using MarsUndiscovered.Messages;

using Newtonsoft.Json;

using Point = SadRogue.Primitives.Point;

namespace MarsUndiscovered.Commands
{
    public class MoveCommand : BaseGameActionCommand<MoveCommandSaveData>
    {
        private IGameObject _gameObject;
        public Tuple<Point, Point> FromTo { get; set; }

        public IGameWorldProvider GameWorldProvider { get; set; }

        public IGameWorld GameWorld => GameWorldProvider.GameWorld;

        public void Initialise(IGameObject gameObject, Tuple<Point, Point> fromTo)
        {
            _gameObject = gameObject;
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
            _gameObject = GameWorld.GameObjects[memento.State.GameObjectId];
        }


        protected override CommandResult ExecuteInternal()
        {
            base.Execute();

            _gameObject.Position = FromTo.Item2;

            Mediator.Send(new MapTileChangedRequest(FromTo.Item1));
            Mediator.Send(new MapTileChangedRequest(FromTo.Item2));

            return CommandResult.Success();
        }

        protected override void UndoInternal()
        {
            _gameObject.Position = FromTo.Item1;
        }
    }
}
