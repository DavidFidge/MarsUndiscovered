using MarsUndiscovered.Components.SaveData;

namespace MarsUndiscovered.Components
{
    public class Floor : Terrain<FloorSaveData>
    {
        public Floor()
        {
        }

        public Floor(uint id) : base(id)
        {
        }
    }
}