using System.Diagnostics;
using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Game.Commands
{
    public class WalkCommand : BaseMarsGameActionCommand
    {
        public Direction Direction { get; set; }
        public Player Player => GameWorld.Player;

        public WalkCommand(IGameWorld gameWorld) : base(gameWorld)
        {
            EndsPlayerTurn = true;
        }

        public void Initialise(Direction direction)
        {
            Debug.Assert(direction != Direction.None, "Use wait command if direction is none");
            Direction = direction;
        }

        protected override CommandResult ExecuteInternal()
        {
            if (Direction == Direction.None)
                return Result(CommandResult.Success(this));

            var playerPosition = Player.Position;
            var newPlayerPosition = Player.Position + Direction;
            var map = Player.CurrentMap;

            Player.RecalculateAttacksForItem(GameWorld.Inventory.EquippedWeapon);

            if (map.Bounds().Contains(newPlayerPosition))
            {
                if (Player.LineAttack != null)
                {
                    var targetPoint = playerPosition + Direction + Direction;
                    var lineAttackPath = Lines.GetLine(playerPosition, targetPoint)
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
                        var command = CommandCollection.CreateCommand<LineAttackCommand>(GameWorld);
                        command.Initialise(Player, lineAttackPath);

                        return Result(CommandResult.Success(this, command));
                    }
                }
                
                if (map.GameObjectCanMove(Player, newPlayerPosition))
                {
                    var command = CommandCollection.CreateCommand<MoveCommand>(GameWorld);

                    command.Initialise(Player, new Tuple<Point, Point>(playerPosition, newPlayerPosition));

                    return Result(CommandResult.Success(this, command));
                }

                var actorAt = map.GetObjectAt<Actor>(newPlayerPosition);
                if (actorAt is Monster)
                {
                    if (GameWorld.ActorAllegiances.RelationshipTo(Player, actorAt) != ActorAllegianceState.Enemy)
                    {
                        var command = CommandCollection.CreateCommand<MeleeAttackCommand>(GameWorld);
                        command.Initialise(Player, actorAt, GameWorld.Inventory.EquippedWeapon);

                        return Result(CommandResult.Success(this, command));
                    }

                    if (Player.MeleeAttack != null)
                    {
                        var command = CommandCollection.CreateCommand<MeleeAttackCommand>(GameWorld);
                        command.Initialise(Player, actorAt, GameWorld.Inventory.EquippedWeapon);

                        return Result(CommandResult.Success(this, command));
                    }
                }

                if (actorAt != null)
                    return Result(CommandResult.Exception(this, $"I bump into a {actorAt.Name}"));

                var terrainAtDestination = map.GetTerrainAt(newPlayerPosition);

                if (terrainAtDestination is Wall)
                {
                    return Result(CommandResult.NoMove(this, "The unrelenting red rock is cold and dry"));
                }

                var mapExitAt = map.GetObjectAt<MapExit>(newPlayerPosition);

                if (mapExitAt != null)
                {
                    var command = CommandCollection.CreateCommand<ChangeMapCommand>(GameWorld);

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
                        return Result(CommandResult.NoMove(this, "I board my ship, make hasty repairs to critical parts and fire the engines! I have escaped!"));
                    }

                    return Result(CommandResult.NoMove(this, "I don't have the parts I need to repair my ship!"));
                }
                
                var machineAt = map.GetObjectAt<Machine>(newPlayerPosition);

                if (machineAt != null)
                {
                    var command = CommandCollection.CreateCommand<ApplyMachineCommand>(GameWorld);
                    command.Initialise(machineAt);

                    return Result(CommandResult.Success(this, command));
                }
            }

            return Result(CommandResult.Exception(this, $"Cannot move {Direction}"));
        }
    }
}
