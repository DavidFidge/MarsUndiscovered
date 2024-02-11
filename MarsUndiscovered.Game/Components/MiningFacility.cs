using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using MarsUndiscovered.Game.Components.SaveData;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components
{
    public class MiningFacility : Indestructible, IMementoState<MiningFacilitySaveData>
    {
        public MiningFacility(IGameWorld gameWorld, uint id) : base(gameWorld, id)
        {
        }

        public MiningFacility WithMiningFacilitySection(char miningFacilitySection)
        {
            _asciiCharacter = miningFacilitySection;
            return this;
        }

        public IMemento<MiningFacilitySaveData> GetSaveState()
        {
            var memento = new Memento<MiningFacilitySaveData>(new MiningFacilitySaveData());

            base.PopulateSaveState(memento.State);
            memento.State.MiningFacilitySection = _asciiCharacter;

            return memento;
        }

        public void SetLoadState(IMemento<MiningFacilitySaveData> memento)
        {
            base.PopulateLoadState(memento.State);
            _asciiCharacter = memento.State.MiningFacilitySection;
        }
    }
}
