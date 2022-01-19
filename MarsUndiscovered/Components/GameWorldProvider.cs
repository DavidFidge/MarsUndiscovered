using System;

using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Services;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Components
{
    public class GameWorldProvider : IGameWorldProvider
    {
        private readonly IFactory<IGameWorld> _gameWorldFactory;

        public IGameWorld GameWorld { get; private set; }

        public GameWorldProvider(IFactory<IGameWorld> gameWorldFactory)
        {
            _gameWorldFactory = gameWorldFactory;
        }

        public SaveGameResult SaveGame(string saveGameName, bool overwrite)
        {
            return GameWorld.SaveGame(saveGameName, overwrite);
        }

        public void CanSaveGame(string saveGameName)
        {
            throw new NotImplementedException();
        }

        public void LoadGame(string gameName)
        {
            GameWorld = _gameWorldFactory.Create();
            GameWorld.LoadGame(gameName);
        }

        public void NewGame(uint? seed = null)
        {
            GameWorld = _gameWorldFactory.Create();
            GameWorld.NewGame(seed);
        }

        public void AfterCreateGame()
        {
            GameWorld.AfterCreateGame();
        }

        public void LoadReplay(string gameName)
        {
            GameWorld = _gameWorldFactory.Create();
            GameWorld.LoadReplay(gameName);
        }
    }
}