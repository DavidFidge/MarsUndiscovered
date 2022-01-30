using System;

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
        public Player Player { get; set; }

        public WalkCommand(IGameWorld gameWorld) : base(gameWorld)
        {
        }

        public void Initialise(Player player, Direction direction)
        {
            Player = player;
            Direction = direction;
        }

        public override IMemento<WalkCommandSaveData> GetSaveState()
        {
            var memento = new Memento<WalkCommandSaveData>(new WalkCommandSaveData());
            base.PopulateSaveState(memento.State);
            memento.State.Direction = Direction;
            return memento;
        }

        public override void SetLoadState(IMemento<WalkCommandSaveData> memento)
        {
            base.PopulateLoadState(memento.State);
            Direction = memento.State.Direction;
            Player = GameWorld.Player;
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
                    var command = CommandFactory.CreateMoveCommand(GameWorld);

                    command.Initialise(Player, new Tuple<Point, Point>(playerPosition, newPlayerPosition));

                    return Result(CommandResult.Success(this, command));
                }

                var terrainAtDestination = map.GetTerrainAt(newPlayerPosition);

                if (terrainAtDestination is Wall)
                {
                    return Result(CommandResult.NoMove(this, "The unrelenting red rock is cold and dry"));
                }

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
                    return Result(CommandResult.Success(this, command));
                }

                var shipAt = map.GetObjectAt<Ship>(newPlayerPosition);

                if (shipAt != null)
                {
                    if (GameWorld.Inventory.HasShipRepairParts)
                    {
                        Player.IsVictorious = true;
                        return Result(CommandResult.NoMove(this, "You board your ship, make hasty repairs to critical parts and fire the engines! You have escaped!"));
                    }

                    return Result(CommandResult.NoMove(this, "You don't have the parts you need to repair your ship!"));
                }
            }

            return Result(CommandResult.Exception(this, $"Cannot move {Direction}"));
        }

        protected override void UndoInternal()
        {
            Player.IsVictorious = false;
        }
    }
}
