using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;
using GoRogue;
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
                if (Player.LineAttack != null)
                {
                    var targetPoint = playerPosition + Direction + Direction;
                    var lineAttackPath = Lines.Get(playerPosition, targetPoint, Lines.Algorithm.BresenhamOrdered)
                        .ToList();

                    // Need to take while monsters or while there is no wall
                    lineAttackPath = lineAttackPath
                        .TakeWhile(p => p == playerPosition || (map.Contains(targetPoint) &&
                                                                (map.GetObjectAt<Monster>(p) != null ||
                                                                 (map.GetObjectAt<Wall>(p) == null &&
                                                                  map.GetObjectAt<Indestructible>(p) == null))))
                        .ToList();

                    if (lineAttackPath.Any(p => map.GetObjectAt<Monster>(p) != null))
                    {
                        var command = CommandFactory.CreateLineAttackCommand(GameWorld);
                        command.Initialise(Player, lineAttackPath);

                        return Result(CommandResult.Success(this, command));
                    }
                }
                
                if (map.GameObjectCanMove(Player, newPlayerPosition))
                {
                    var command = CommandFactory.CreateMoveCommand(GameWorld);

                    command.Initialise(Player, new Tuple<Point, Point>(playerPosition, newPlayerPosition));

                    return Result(CommandResult.Success(this, command));
                }

                var actorAt = map.GetObjectAt<Actor>(newPlayerPosition);
                if (actorAt is Monster)
                {
                    if (Player.MeleeAttack != null)
                    {
                        var command = CommandFactory.CreateMeleeAttackCommand(GameWorld);
                        command.Initialise(Player, actorAt);

                        return Result(CommandResult.Success(this, command));
                    }
                }

                if (actorAt != null)
                    return Result(CommandResult.Exception(this, $"You bump into a {actorAt.Name}"));

                var terrainAtDestination = map.GetTerrainAt(newPlayerPosition);

                if (terrainAtDestination is Wall)
                {
                    return Result(CommandResult.NoMove(this, "The unrelenting red rock is cold and dry"));
                }

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
                        GameWorld.Morgue.GameEnded();
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
