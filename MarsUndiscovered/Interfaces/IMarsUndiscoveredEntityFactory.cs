using MarsUndiscovered.Graphics.Models;

namespace MarsUndiscovered.Interfaces
{
    public interface IMarsUndiscoveredEntityFactory
    {
        MarsUndiscoveredEntity Create();
        void Release(MarsUndiscoveredEntity marsUndiscoveredEntity);
    }
}