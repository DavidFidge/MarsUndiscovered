using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Commands
{
    public class WaitCommand : BaseMarsGameActionCommand<WaitCommandSaveData>
    {
        public Player Player { get; set; }

        public WaitCommand(IGameWorld gameWorld) : base(gameWorld)
        {
            EndsPlayerTurn = true;
            PersistForReplay = true;
        }

        public void Initialise(Player player)
        {
            Player = player;
        }

        public override IMemento<WaitCommandSaveData> GetSaveState()
        {
            var memento = new Memento<WaitCommandSaveData>(new WaitCommandSaveData());
            base.PopulateSaveState(memento.State);
            return memento;
        }

        public override void SetLoadState(IMemento<WaitCommandSaveData> memento)
        {
            base.PopulateLoadState(memento.State);
            Player = GameWorld.Player;
        }

        protected override CommandResult ExecuteInternal()
        {
            // Currently wait commands do nothing
            return Result(CommandResult.Success(this));
        }

        protected override void UndoInternal()
        {
        }
    }
}
