using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Interfaces;

using Newtonsoft.Json;

namespace MarsUndiscovered.Game.Commands
{
    public abstract class BaseMarsGameActionCommand<T> : BaseGameActionCommand, IBaseMarsGameActionCommand, IMementoState<T>
    where T : BaseCommandSaveData, new()
    {
        protected T _data = new();

        protected T CloneData()
        {
            return (T)_data.Clone(); 
        }
        
        [JsonIgnore]
        public IGameWorld GameWorld { get; private set; }
        
        [JsonIgnore]
        public ICommandCollection CommandCollection { get; set; }

        public BaseMarsGameActionCommand(IGameWorld gameWorld)
        {
            GameWorld = gameWorld;
        }
        
        public virtual IMemento<T> GetSaveState()
        {
            var memento = new Memento<T>(CloneData());
        
            memento.State.AdvanceSequenceNumber = AdvanceSequenceNumber;
            memento.State.Id = Id;
            memento.State.TurnDetails = (TurnDetails)TurnDetails.Clone();

            return memento;
        }

        public virtual void SetLoadState(IMemento<T> memento)
        {
            _data = (T)memento.State.Clone();
            _data.TurnDetails = (TurnDetails)memento.State.TurnDetails.Clone();
            TurnDetails = (TurnDetails)memento.State.TurnDetails.Clone();
            AdvanceSequenceNumber = _data.AdvanceSequenceNumber;
            Id = _data.Id;
        }

        public void ApplyWeaknesses(Actor source, Actor target)
        {
            if (source.CanConcuss)
                target.ApplyConcussion();
        }
    }
}
