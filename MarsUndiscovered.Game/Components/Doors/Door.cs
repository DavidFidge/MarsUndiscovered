using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using MarsUndiscovered.Game.Components.SaveData;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components
{
    public class Door : MarsGameObject, IMementoState<DoorSaveData>
    {
        public DoorType DoorType { get; set; }
        public bool IsOpen { get; set; }

        public Door(IGameWorld gameWorld, uint id) : base(gameWorld, Constants.DoorLayer, true, false, idGenerator: () => id)
        {
        }

        public IMemento<DoorSaveData> GetSaveState()
        {
            var memento = new Memento<DoorSaveData>(new DoorSaveData());

            base.PopulateSaveState(memento.State);
            memento.State.DoorTypeName = DoorType.Name;
            memento.State.IsOpen = IsOpen;

            return memento;
        }

        public void SetLoadState(IMemento<DoorSaveData> memento)
        {
            base.PopulateLoadState(memento.State);
            DoorType = DoorType.DoorTypes[memento.State.DoorTypeName];
            IsOpen = memento.State.IsOpen;
        }

        public void Open()
        {
            if (IsOpen)
                return;
            
            DoorType = DoorType.ClosedOpenMapping[DoorType];
            IsOpen = true;
        }
        
        public void Closed()
        {
            if (!IsOpen)
                return;
            
            DoorType = DoorType.OpenClosedMapping[DoorType];
            IsOpen = false;
        }
    }
}