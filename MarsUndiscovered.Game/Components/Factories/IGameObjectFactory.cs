using GoRogue.GameFramework;

using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components.Factories
{
    public interface IGameObjectFactory : ISaveable
    {
        void Initialise(IGameWorld gameWorld);
        uint LastId { get; }
        IDictionary<uint, IGameObject> GameObjects { get; }
        T CreateGameObject<T>() where T : IGameObject;
        T CreateGameObject<T>(uint id) where T : IGameObject;
    }
}
