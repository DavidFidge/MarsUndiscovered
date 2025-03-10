using FrigidRogue.MonoGame.Core.Extensions;
using FrigidRogue.WaveFunctionCollapse;
using FrigidRogue.WaveFunctionCollapse.ContentLoaders;
using FrigidRogue.WaveFunctionCollapse.Renderers;
using GoRogue.MapGeneration;
using GoRogue.Random;
using MarsUndiscovered.Game.Components.Maps;
using Microsoft.Xna.Framework;
using SadRogue.Primitives.GridViews;
using ShaiRandom.Generators;

namespace MarsUndiscovered.Game.Components.GenerationSteps;

public class MiningFacilityGeneration : GenerationStep
{
    private readonly IWaveFunctionCollapseGeneratorPasses _waveFunctionCollapseGeneratorPasses;
    private readonly IWaveFunctionCollapseGeneratorPassesContentLoader _contentLoader;
    private readonly IWaveFunctionCollapseGeneratorPassesRenderer _renderer;
    
    public IEnhancedRandom RNG { get; set; } = GlobalRandom.DefaultRNG;

    /// <summary>
    /// Creates a new mining facility generation step which creates buildings for the mining facility
    /// </summary>
    /// <param name="name">The name of the generation step.  Defaults to <see cref="MiningFacility" />.</param>
    public MiningFacilityGeneration(
        IWaveFunctionCollapseGeneratorPasses waveFunctionCollapseGeneratorPasses,
        IWaveFunctionCollapseGeneratorPassesContentLoader contentLoader,
        IWaveFunctionCollapseGeneratorPassesRenderer renderer,
        string name = null)
        : base(name)
    {
        _waveFunctionCollapseGeneratorPasses = waveFunctionCollapseGeneratorPasses;
        _contentLoader = contentLoader;
        _renderer = renderer;
    }

    protected override IEnumerator<object> OnPerform(GenerationContext generationContext)
    {
        _waveFunctionCollapseGeneratorPasses.LoadContent(_contentLoader, "Maps/MiningFacility");

        var mapSize = new Point(generationContext.Width, generationContext.Height);
        
        // WaveFunctionCollapse being used here are images with 3x3 pixels representing a square on the map.
        // So the wave function collapse can only generate a texture divisible by 3
        var textureSize = new Point(
            generationContext.Width / _waveFunctionCollapseGeneratorPasses.TileWidth,
            generationContext.Height / _waveFunctionCollapseGeneratorPasses.TileHeight);

        _waveFunctionCollapseGeneratorPasses.MapOptions.MapWidth = textureSize.X;
        _waveFunctionCollapseGeneratorPasses.MapOptions.MapHeight = textureSize.Y;
        _waveFunctionCollapseGeneratorPasses.CreatePasses();

        var isRejected = true;

        ArrayView<GameObjectType> arrayView = null;
        
        while (isRejected)
        {
            _waveFunctionCollapseGeneratorPasses.Reset();
            _waveFunctionCollapseGeneratorPasses.ExecuteUntilSuccess();

            var texture2D = _waveFunctionCollapseGeneratorPasses.RenderToTexture2D(_renderer);

            var data = new Color[texture2D.Width * texture2D.Height];
            texture2D.GetData(data);

            if (mapSize != textureSize)
            {
                var dataMapSize = new Color[mapSize.X * mapSize.Y];
                Array.Fill(dataMapSize, Color.White);
                data.CopyInto(dataMapSize, texture2D.Width, mapSize.X);
                data = dataMapSize;
            }
            
            arrayView = new ArrayView<GameObjectType>(generationContext.Width, generationContext.Height);

            var miningFacilityFloorCount = 0;
            var stockpileWallCount = 0;
            
            for (var index = 0; index < data.Length; index++)
            {
                GameObjectType gameObjectType;

                if (data[index].Equals(Color.Black))
                {
                    gameObjectType = WallType.MiningFacilityWall;
                }
                else if (data[index].Equals(Color.Blue))
                {
                    gameObjectType = FloorType.MiningFacilityFloor;
                    miningFacilityFloorCount++;
                }
                else if (data[index].Equals(Color.White))
                {
                    gameObjectType = FloorType.RockFloor;
                }
                else if (data[index].Equals(Color.Red))
                {
                    gameObjectType = WallType.StockpileWall;
                    stockpileWallCount++;
                }
                else
                {
                    throw new Exception("Unknown colour in mining facility tiles");
                }

                arrayView[index] = gameObjectType;
            }

            if (miningFacilityFloorCount / (float)arrayView.Count >= 0.3f &&
                stockpileWallCount / (float)arrayView.Count >= 0.1f)
            {
                isRejected = false;
            }
        }

        generationContext.Add(arrayView, MapGenerator.WallFloorTypeTag);

        yield return null;
    }
}