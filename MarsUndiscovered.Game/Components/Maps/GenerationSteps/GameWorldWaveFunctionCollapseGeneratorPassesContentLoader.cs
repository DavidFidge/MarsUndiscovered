using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.WaveFunctionCollapse.ContentLoaders;
using FrigidRogue.WaveFunctionCollapse.Options;
using Microsoft.Xna.Framework.Graphics;

namespace MarsUndiscovered.Game.Components.GenerationSteps;

public class GameWorldWaveFunctionCollapseGeneratorPassesContentLoader : IWaveFunctionCollapseGeneratorPassesContentLoader
{
    private readonly IGameProvider _gameProvider;

    private WaveFunctionCollapseGeneratorPassesContentLoader _waveFunctionCollapseGeneratorPassesContentLoader;
    public Rules Rules => _waveFunctionCollapseGeneratorPassesContentLoader.Rules;
    public Dictionary<string, Texture2D> Textures => _waveFunctionCollapseGeneratorPassesContentLoader.Textures;


    public GameWorldWaveFunctionCollapseGeneratorPassesContentLoader(IGameProvider gameProvider)
    {
        _gameProvider = gameProvider;
    }

    public void LoadContent(string content)
    {
        _waveFunctionCollapseGeneratorPassesContentLoader ??= new WaveFunctionCollapseGeneratorPassesContentLoader(_gameProvider.Game.Content);
        _waveFunctionCollapseGeneratorPassesContentLoader.LoadContent(content);
    }
}