using FrigidRogue.MonoGame.Core.Services;

namespace MarsUndiscovered.Interfaces
{
    public interface IGameWorldEndpoint
    {
        // TODO - change all direct usages of GameWorld by the view to methods
        IGameWorld GameWorld { get; }
        SaveGameResult SaveGame(string saveGameName, bool overwrite);
        void CanSaveGame(string saveGameName);
        public void LoadGame(string filename);
        public void LoadReplay(string filename);
        void NewGame(uint? seed = null);
        void AfterCreateGame();
    }
}