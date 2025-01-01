using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;
using MarsUndiscovered.Game.Components.SaveData;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components
{
    public class Feature : MarsGameObject, IMementoState<FeatureSaveData>
    {
        public FeatureType FeatureType { get; set; }
        
        public override char AsciiCharacter => FeatureType.AsciiCharacter;
        
        public Feature(IGameWorld gameWorld, uint id) : base(gameWorld, Constants.FeatureLayer, idGenerator: () => id)
        {
        }
        
        public IMemento<FeatureSaveData> GetSaveState()
        {
            var memento = new Memento<FeatureSaveData>(new FeatureSaveData());

            PopulateSaveState(memento.State);
            memento.State.FeatureTypeName = FeatureType.Name;

            return memento;
        }

        public void SetLoadState(IMemento<FeatureSaveData> memento)
        {
            PopulateLoadState(memento.State);
            FeatureType = FeatureType.FeatureTypes[memento.State.FeatureTypeName];
        }

        public Feature WithFeatureType(FeatureType featureType)
        {
            FeatureType = featureType;

            return this;
        }

        public string GetAmbientText()
        {
            return FeatureType.GetAmbientText();
        }
    }
}