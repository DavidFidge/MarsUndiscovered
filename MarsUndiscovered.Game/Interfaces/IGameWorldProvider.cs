using System.Threading.Tasks;
using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Services;
using GoRogue.GameFramework;
using GoRogue.Pathing;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Dto;
using Microsoft.Xna.Framework.Input;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Interfaces
{
    public interface IGameWorldProvider
    {
        void LoadGame(string filename);
        void NewGame(ulong? seed = null);
        ProgressiveWorldGenerationResult ProgressiveWorldGeneration(ulong? seed, int step, WorldGenerationTypeParams worldGenerationTypeParams);
        IGameWorld GameWorld { get; }
    }
}
