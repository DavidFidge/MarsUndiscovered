namespace MarsUndiscovered.Components
{
    public class Terrain : MarsGameObject
    {
        public Terrain(bool isWalkable = true, bool isTransparent = true) : base(0, isWalkable, isTransparent)
        {
        }

        public Terrain(uint id, bool isWalkable = true, bool isTransparent = true) : base(0, isWalkable, isTransparent, idGenerator: () => id)
        {
        }
    }
}