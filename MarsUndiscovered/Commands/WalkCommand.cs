using System;
using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using GoRogue.GameFramework;

using MarsUndiscovered.Components;
using MarsUndiscovered.Interfaces;

using SadRogue.Primitives;

namespace MarsUndiscovered.Commands
{
    public class WalkCommand : BaseGameActionCommand<WalkCommandSaveData>
    {
        public Direction Direction { get; set; }
        private Player _player;

        public IFactory<MoveCommand> MoveCommandFactory { get; set; }
        public IFactory<AttackCommand> AttackCommandFactory { get; set; }
        public IGameWorldProvider GameWorldProvider { get; set; }
        public IGameWorld GameWorld => GameWorldProvider.GameWorld;
        public Map Map => GameWorldProvider.GameWorld.Map;

        public void Initialise(Player player, Direction direction)
        {
            _player = player;
            Direction = direction;
        }

        public override IMemento<WalkCommandSaveData> GetSaveState(IMapper mapper)
        {
            return Memento<WalkCommandSaveData>.CreateWithAutoMapper(this, mapper);
        }

        public override void SetLoadState(IMemento<WalkCommandSaveData> memento, IMapper mapper)
        {
            base.SetLoadState(memento, mapper);

            Memento<WalkCommandSaveData>.SetWithAutoMapper(this, memento, mapper);
            _player = (Player)GameWorld.GameObjects[memento.State.GameObjectId];
        }

        protected override CommandResult ExecuteInternal()
        {
            var playerPosition = _player.Position;
            var newPlayerPosition = _player.Position + Direction;

            if (Map.GameObjectCanMove(_player, newPlayerPosition))
            {
                var terrainAtDestination = Map.GetTerrainAt(newPlayerPosition);

                if (terrainAtDestination is Floor)
                {
                    var gameObjectsAt = Map.GetObjectsAt(newPlayerPosition);

                    var actorAt = gameObjectsAt.FirstOrDefault(go => go is Actor) as Actor;

                    if (actorAt == null)
                    {
                        var command = MoveCommandFactory.Create();

                        command.Initialise(_player, new Tuple<Point, Point>(playerPosition, newPlayerPosition));

                        return Result(CommandResult.Success(command));
                    }
                    if (actorAt is Monster)
                    {
                        var command = AttackCommandFactory.Create();
                        command.Initialise(_player, actorAt);

                        return Result(CommandResult.Success(command));
                    }

                    return Result(CommandResult.Failure($"You bump into a {actorAt.Name}"));
                }
                if (terrainAtDestination is Wall)
                {
                    return Result(CommandResult.Failure("The unrelenting red rock is cold and dry"));
                }
            }

            return Result(CommandResult.Failure($"Cannot move {Direction}"));
        }

        protected override void UndoInternal()
        {
        }
    }
}
