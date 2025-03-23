using System.Threading.Tasks;
using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Services;
using GoRogue.GameFramework;
using GoRogue.Pathing;
using MarsUndiscovered.Game.Components.Dto;
using MarsUndiscovered.Interfaces;
using Microsoft.Xna.Framework.Input;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using Point = SadRogue.Primitives.Point;
using Rectangle = SadRogue.Primitives.Rectangle;

namespace MarsUndiscovered.Game.Components
{
    public class GameWorldProvider : IGameWorldProvider
    {
        private readonly IFactory<IGameWorld> _gameWorldFactory;

        public IGameWorld GameWorld { get; private set; }

        public GameWorldProvider(IFactory<IGameWorld> gameWorldFactory)
        {
            _gameWorldFactory = gameWorldFactory;
        }

        public void LoadGame(string gameName)
        {
            GameWorld = _gameWorldFactory.Create();
            GameWorld.LoadGame(gameName);
        }

        public void NewGame(ulong? seed = null)
        {
            GameWorld = _gameWorldFactory.Create();
            GameWorld.NewGame(seed);
        }

        public ProgressiveWorldGenerationResult ProgressiveWorldGeneration(ulong? seed, int step, WorldGenerationTypeParams worldGenerationTypeParams)
        {
            GameWorld = _gameWorldFactory.Create();
            return GameWorld.ProgressiveWorldGeneration(seed, step, worldGenerationTypeParams);
        }
    }
}
