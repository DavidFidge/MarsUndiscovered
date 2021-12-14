namespace MarsUndiscovered.Components
{
    public interface IFactory<T>
    {
        T Create();
        void Release(T mapTileEntity);
    }
}