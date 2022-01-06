using System;
using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using GoRogue.GameFramework;

using MarsUndiscovered.Components;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Interfaces;
using Newtonsoft.Json;
using SadRogue.Primitives;

namespace MarsUndiscovered.Commands
{
    public class WalkCommand : BaseMarsGameActionCommand<WalkCommandSaveData>
    {
        public Direction Direction { get; set; }
        public Player Player { get; private set; }

        [JsonIgnore]
        public ICommandFactory CommandFactory { get; set; }

        protected Map Map => GameWorld.Map;

        public void Initialise(Player player, Direction direction)
        {
            Player = player;
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
            Player = (Player)GameWorld.GameObjects[memento.State.PlayerId];
        }

        protected override CommandResult ExecuteInternal()
        {
            var playerPosition = Player.Position;
            var newPlayerPosition = Player.Position + Direction;

            if (Map.GameObjectCanMove(Player, newPlayerPosition))
            {
                var terrainAtDestination = Map.GetTerrainAt(newPlayerPosition);

                if (terrainAtDestination is Floor)
                {
                    var gameObjectsAt = Map.GetObjectsAt(newPlayerPosition);

                    var actorAt = gameObjectsAt.FirstOrDefault(go => go is Actor) as Actor;

                    if (actorAt == null)
                    {
                        var command = CommandFactory.CreateMoveCommand(GameWorld);

                        command.Initialise(Player, new Tuple<Point, Point>(playerPosition, newPlayerPosition));

                        return Result(CommandResult.Success(command));
                    }
                    if (actorAt is Monster)
                    {
                        var command = CommandFactory.CreateAttackCommand(GameWorld);
                        command.Initialise(Player, actorAt);

                        return Result(CommandResult.Success(command));
                    }

                    return Result(CommandResult.Failure($"You bump into a {actorAt.Name}"));
                }
                if (terrainAtDestination is Wall)
                {
                    return Result(CommandResult.NoMove("The unrelenting red rock is cold and dry"));
                }
            }

            return Result(CommandResult.Failure($"Cannot move {Direction}"));
        }

        protected override void UndoInternal()
        {
        }
    }
}
