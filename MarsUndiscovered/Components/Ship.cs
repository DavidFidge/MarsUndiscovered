using AutoMapper;

using FrigidRogue.MonoGame.Core.Interfaces.Components;

using MarsUndiscovered.Components.SaveData;

namespace MarsUndiscovered.Components
{
    public class Ship : Indestructible, IMementoState<ShipSaveData>
    {
        public char ShipPart { get; set; }

        public Ship(uint id) : base(id)
        {
        }

        public Ship WithShipPart(char shipPart)
        {
            ShipPart = shipPart;
            return this;
        }

        public IMemento<ShipSaveData> GetSaveState(IMapper mapper)
        {
            return CreateWithAutoMapper<ShipSaveData>(mapper);
        }

        public void SetLoadState(IMemento<ShipSaveData> memento, IMapper mapper)
        {
            SetWithAutoMapper<ShipSaveData>(memento, mapper);
        }
    }
}