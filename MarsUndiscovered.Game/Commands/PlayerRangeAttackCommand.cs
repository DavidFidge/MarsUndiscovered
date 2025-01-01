using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;
using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Commands
{
    public class PlayerRangeAttackCommand : BaseMarsGameActionCommand<PlayerRangeAttackCommandSaveData>
    {
        public Point TargetPoint => _data.TargetPoint;
        public Player Player => GameWorld.Player;
        public Item Item => GameWorld.Items[_data.ItemId];
        
        public PlayerRangeAttackCommand(IGameWorld gameWorld) : base(gameWorld)
        {
            EndsPlayerTurn = true;
        }

        public void Initialise(Point targetPoint, Item item)
        {
            _data.ItemId = item.ID;
            _data.TargetPoint = targetPoint;
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

        protected override void UndoInternal()
        {
            Player.IsVictorious = false;
        }
    }
}
