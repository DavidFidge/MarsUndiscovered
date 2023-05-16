using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.WaveFunctionCollapse;
using FrigidRogue.WaveFunctionCollapse.Options;
using FrigidRogue.WaveFunctionCollapse.Renderers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MarsUndiscovered.Game.Components.GenerationSteps;

public class GameWorldWaveFunctionCollapseGeneratorPassesRenderer : IWaveFunctionCollapseGeneratorPassesRenderer
{
    private readonly IGameProvider _gameProvider;
    private WaveFunctionCollapseGeneratorPassesRenderer _waveFunctionCollapseGeneratorPassesRenderer;
    private SpriteBatch _spriteBatch;

    public GameWorldWaveFunctionCollapseGeneratorPassesRenderer(IGameProvider gameProvider)
    {
        _gameProvider = gameProvider;
    }

    public Texture2D RenderToTexture2D(TileResult[] tiles, MapOptions mapOptions, Rectangle tileTextureSize)
    {
        _spriteBatch ??= new SpriteBatch(_gameProvider.Game.GraphicsDevice);

        _waveFunctionCollapseGeneratorPassesRenderer ??= new WaveFunctionCollapseGeneratorPassesRenderer(_gameProvider.Game.GraphicsDevice, _spriteBatch);

        return _waveFunctionCollapseGeneratorPassesRenderer.RenderToTexture2D(tiles, mapOptions, tileTextureSize);
    }
}