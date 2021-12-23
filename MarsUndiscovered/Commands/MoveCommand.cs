using System;
using System.Collections.Generic;
using System.Text;

using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using GoRogue.GameFramework;

using MarsUndiscovered.Interfaces;

using Newtonsoft.Json;

using Point = SadRogue.Primitives.Point;

namespace MarsUndiscovered.Commands
{
    public class MoveCommand : BaseGameActionCommand<MoveCommandData>
    {
        private IGameObject _gameObject;
        private Tuple<Point, Point> _fromTo { get; set; }

        [JsonIgnore]
        public IGameWorld GameWorld { get; set; }

        public void Initialise(IGameObject gameObject, Tuple<Point, Point> fromTo)
        {
            _gameObject = gameObject;
            _fromTo = fromTo;
        }

        public override IMemento<MoveCommandData> GetState()
        {
            return new Memento<MoveCommandData>(new MoveCommandData { FromTo = _fromTo, GameObjectId = _gameObject.ID });
        }

        public override void SetState(IMemento<MoveCommandData> memento)
        {
            Initialise(GameWorld.GameObjects[memento.State.GameObjectId], memento.State.FromTo);
            GameWorld.GameTurnService.Populate(TurnDetails);
        }

        public override void Execute()
        {
            GameWorld.Move(_gameObject, _fromTo.Item2);
        }

        public override void Undo()
        {
            GameWorld.Move(_gameObject, _fromTo.Item1);
        }
    }
}
