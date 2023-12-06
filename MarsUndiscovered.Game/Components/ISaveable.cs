using FrigidRogue.MonoGame.Core.Interfaces.Services;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components
{
    public interface ISaveable
    {
        public void SaveState(ISaveGameService saveGameService, IGameWorld gameWorld);
        public void LoadState(ISaveGameService saveGameService, IGameWorld gameWorld);
    }
}