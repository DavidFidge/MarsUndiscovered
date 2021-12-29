using MarsUndiscovered.Components.SaveData;

namespace MarsUndiscovered.Components
{
    public abstract class Terrain<T> : MarsGameObject<T> where T : GameObjectSaveData
    {
        public Terrain(bool isWalkable = true, bool isTransparent = true) : base(0, isWalkable, isTransparent)
        {
        }

        public Terrain(uint id, bool isWalkable = true, bool isTransparent = true) : base(0, isWalkable, isTransparent, idGenerator: () => id)
        {
        }
    }
}