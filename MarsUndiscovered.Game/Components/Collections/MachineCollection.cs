using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.SaveData;

namespace MarsUndiscovered.Game.Components
{
    public class MachineCollection : GameObjectCollection<Machine, MachineSaveData>
    {
        public MachineCollection(IGameObjectFactory gameObjectFactory) : base(gameObjectFactory)
        {
        }
    }
}