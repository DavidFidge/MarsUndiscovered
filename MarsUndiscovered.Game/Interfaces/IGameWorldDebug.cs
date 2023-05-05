using MarsUndiscovered.Game.Components;

namespace MarsUndiscovered.Interfaces
{
    public interface IGameWorldDebug : IGameWorldConsoleCommandEndpoint
    {
        void Initialise(GameWorld gameWorld);
    }
}