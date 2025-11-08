using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;
using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Commands
{
    public class PlayerRangeAttackCommand : BaseMarsGameActionCommand
    {
        public Point TargetPoint { get; set; }
        public Player Player => GameWorld.Player;
        public Item Item { get; set; }

        public PlayerRangeAttackCommand(IGameWorld gameWorld) : base(gameWorld)
        {
            EndsPlayerTurn = true;
        }

        public void Initialise(Point targetPoint, Item item)
        {
            Item = item;
            TargetPoint = targetPoint;
        }

        protected override CommandResult ExecuteInternal()
        {
            Player.RecalculateAttacksForItem(Item);
            
            if (Player.LaserAttack != null)
            {
                var laserAttackCommand = CommandCollection.CreateCommand<LaserAttackCommand>(GameWorld);
                laserAttackCommand.Initialise(Player, TargetPoint);
                
                
                return Result(CommandResult.Success(this, laserAttackCommand));
            }

            EndsPlayerTurn = false;
            
            return Result(CommandResult.Exception(this, "Ranged weapon not equipped"));
        }
    }
}
