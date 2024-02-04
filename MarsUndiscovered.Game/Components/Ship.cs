using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using MarsUndiscovered.Game.Components.SaveData;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components
{
    public class Ship : Indestructible, IMementoState<ShipSaveData>
    {
        public Ship(IGameWorld gameWorld, uint id) : base(gameWorld, id)
        {
        }

        public Ship WithShipPart(char shipPart)
        {
            _asciiCharacter = shipPart;
            return this;
        }

        public IMemento<ShipSaveData> GetSaveState()
        {
            var memento = new Memento<ShipSaveData>(new ShipSaveData());

            base.PopulateSaveState(memento.State);
            memento.State.ShipPart = _asciiCharacter;

            return memento;
        }

        public void SetLoadState(IMemento<ShipSaveData> memento)
        {
            base.PopulateLoadState(memento.State);
            _asciiCharacter = memento.State.ShipPart;
        }
    }
}
