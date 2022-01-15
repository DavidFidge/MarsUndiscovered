using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Components
{
    public interface ICommandFactory<T>
    {
        T Create(IGameWorld gameWorld);
        void Release(T command);
    }
}