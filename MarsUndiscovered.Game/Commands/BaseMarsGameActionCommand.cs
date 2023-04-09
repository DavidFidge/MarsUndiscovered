using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Interfaces;

using Newtonsoft.Json;

namespace MarsUndiscovered.Game.Commands
{
    public abstract class BaseMarsGameActionCommand<T> : BaseGameActionCommand, IBaseMarsGameActionCommand, IMementoState<T>
    {
        [JsonIgnore]
        public IGameWorld GameWorld { get; private set; }

        [JsonIgnore]
        public ICommandFactory CommandFactory { get; set; }

        public virtual bool InterruptsMovement => false;

        public BaseMarsGameActionCommand(IGameWorld gameWorld)
        {
            GameWorld = gameWorld;
        }

        protected void PopulateSaveState(BaseCommandSaveData state)
        {
            state.AdvanceSequenceNumber = AdvanceSequenceNumber;
            state.TurnDetails = (TurnDetails)TurnDetails.Clone();
        }

        protected void PopulateLoadState(BaseCommandSaveData state)
        {
            AdvanceSequenceNumber = state.AdvanceSequenceNumber;
            TurnDetails = (TurnDetails)state.TurnDetails.Clone();
        }
        
        public abstract void SetLoadState(IMemento<T> memento);
        public abstract IMemento<T> GetSaveState();
    }
}
