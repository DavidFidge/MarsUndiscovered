using MarsUndiscovered.Components.SaveData;

namespace MarsUndiscovered.Components
{
    public abstract class Terrain : MarsGameObject
    {
        public Terrain(uint id, bool isWalkable = true, bool isTransparent = true) : base(0, isWalkable, isTransparent, idGenerator: () => id)
        {
        }

        public int CreatedIndex { get; set; }
    }
}