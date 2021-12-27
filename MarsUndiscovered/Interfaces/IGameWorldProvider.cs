using FrigidRogue.MonoGame.Core.Services;

namespace MarsUndiscovered.Interfaces
{
    public interface IGameWorldProvider
    {
        IGameWorld GameWorld { get; set; }
        SaveGameResult SaveGame(string saveGameName, bool overwrite);
        void CanSaveGame(string saveGameName);
        public void LoadGame(string gameName);
        void NewGame(uint? seed = null);
    }
}