using FrigidRogue.MonoGame.Core.Interfaces.Components;

namespace MarsUndiscovered.Components.SaveData
{
    public class MonsterSaveData : ActorSaveData
    {
        public string BreedName { get; set; }
        public IList<IMemento<SeenTileSaveData>> SeenTiles { get; set; }
    }
}
