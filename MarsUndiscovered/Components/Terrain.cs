namespace MarsUndiscovered.Components
{
    public abstract class Terrain : MarsGameObject
    {
        public bool IsDestroyed { get; set; }
        public bool IsDestroyable { get; set; }

        public Terrain(uint id, bool isWalkable = true, bool isTransparent = true) : base(Constants.TerrainLayer, isWalkable, isTransparent, idGenerator: () => id)
        {
            IsDestroyable = true;
        }

        public int Index { get; set; }
    }
}