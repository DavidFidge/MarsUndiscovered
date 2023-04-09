using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components
{
    public interface ICommandFactory<T>
    {
        T Create(IGameWorld gameWorld);
        void Release(T command);
    }
}