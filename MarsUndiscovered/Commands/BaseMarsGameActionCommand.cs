using FrigidRogue.MonoGame.Core.Components;

using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Interfaces;

using Newtonsoft.Json;

namespace MarsUndiscovered.Commands
{
    public abstract class BaseMarsGameActionCommand<T> : BaseStatefulGameActionCommand<T>
    {
        [JsonIgnore]
        public IGameWorld GameWorld { get; private set; }

        [JsonIgnore]
        public ICommandFactory CommandFactory { get; set; }

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
    }
}
