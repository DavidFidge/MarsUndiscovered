using System;

using AutoMapper;

using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using MarsUndiscovered.Components;
using MarsUndiscovered.Interfaces;

using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Commands
{
    public class WalkCommand : BaseMarsGameActionCommand<WalkCommandSaveData>
    {
        public Direction Direction { get; set; }
        public Player Player { get; private set; }

        public WalkCommand(IGameWorld gameWorld) : base(gameWorld)
        {
        }

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
            if (Direction == Direction.None)
                return Result(CommandResult.Success(this));

            var playerPosition = Player.Position;
            var newPlayerPosition = Player.Position + Direction;
            var map = Player.CurrentMap;

            if (map.Bounds().Contains(newPlayerPosition))
            {
                if (map.GameObjectCanMove(Player, newPlayerPosition))
                {
                    var terrainAtDestination = map.GetTerrainAt(newPlayerPosition);

                    if (terrainAtDestination is Floor)
                    {
                        var command = CommandFactory.CreateMoveCommand(GameWorld);

                        command.Initialise(Player, new Tuple<Point, Point>(playerPosition, newPlayerPosition));

                        return Result(CommandResult.Success(this, command));

                    }

                    if (terrainAtDestination is Wall)
                    {
                        return Result(CommandResult.NoMove(this, "The unrelenting red rock is cold and dry"));
                    }
                }
                else
                {
                    // Maps get created with the setting where there cannot be multiple objects in the same layer.
                    // Thus if player cannot move to the new position then a monster should be there.
                    var actorAt = map.GetObjectAt<Actor>(newPlayerPosition);

                    if (actorAt is Monster)
                    {
                        var command = CommandFactory.CreateAttackCommand(GameWorld);
                        command.Initialise(Player, actorAt);

                        return Result(CommandResult.Success(this, command));
                    }

                    if (actorAt != null)
                        return Result(CommandResult.Exception(this, $"You bump into a {actorAt.Name}"));

                    var mapExitAt = map.GetObjectAt<MapExit>(newPlayerPosition);

                    if (mapExitAt != null)
                    {
                        var command = CommandFactory.CreateChangeMapCommand(GameWorld);

                        command.Initialise(Player, mapExitAt);

                        var exitText = mapExitAt.Direction == Direction.Down ? "descend" : "ascend";

                        return Result(CommandResult.Success(this, $"You {exitText}", command));
                    }
                }
            }

            return Result(CommandResult.Exception(this, $"Cannot move {Direction}"));
        }

        protected override void UndoInternal()
        {
        }
    }
}
