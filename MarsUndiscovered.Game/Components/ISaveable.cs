using FrigidRogue.MonoGame.Core.Interfaces.Services;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components
{
    public interface ISaveable
    {
        void SaveState(ISaveGameService saveGameService, IGameWorld gameWorld);
        void LoadState(ISaveGameService saveGameService, IGameWorld gameWorld);
    }
}