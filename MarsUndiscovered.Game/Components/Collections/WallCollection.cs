using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.SaveData;

namespace MarsUndiscovered.Game.Components
{
    public class WallCollection : GameObjectCollection<Wall, WallSaveData>
    {
        public WallCollection(IGameObjectFactory gameObjectFactory) : base(gameObjectFactory)
        {
        }
    }
}