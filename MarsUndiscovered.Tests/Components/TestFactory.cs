using FrigidRogue.MonoGame.Core.Components;

namespace MarsUndiscovered.Tests.ViewModels
{
    public class TestFactory<T> : IFactory<T> where T : class, new()
    {
        public T Create()
        {
            return new T();
        }

        public void Release(T mapTileEntity)
        {
        }
    }
}