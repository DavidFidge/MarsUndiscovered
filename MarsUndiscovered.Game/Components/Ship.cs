using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using MarsUndiscovered.Game.Components.SaveData;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components
{
    public class Ship : Indestructible, IMementoState<ShipSaveData>
    {
        public char ShipPart { get; set; }

        public Ship(IGameWorld gameWorld, uint id) : base(gameWorld, id)
        {
        }

        public Ship WithShipPart(char shipPart)
        {
            ShipPart = shipPart;
            return this;
        }

        public IMemento<ShipSaveData> GetSaveState()
        {
            var memento = new Memento<ShipSaveData>(new ShipSaveData());

            base.PopulateSaveState(memento.State);
            memento.State.ShipPart = ShipPart;

            return memento;
        }

        public void SetLoadState(IMemento<ShipSaveData> memento)
        {
            base.PopulateLoadState(memento.State);
            ShipPart = memento.State.ShipPart;
        }
    }
}
