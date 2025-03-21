using FrigidRogue.MonoGame.Core.Interfaces.Components;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.SaveData;

namespace MarsUndiscovered.Game.Components
{
    public class EnvironmentalEffectCollection : GameObjectCollection<EnvironmentalEffect, EnvironmentalEffectSaveData>
    {
        public EnvironmentalEffectCollection(IGameObjectFactory gameObjectFactory) : base(gameObjectFactory)
        {
        }
    }
}
