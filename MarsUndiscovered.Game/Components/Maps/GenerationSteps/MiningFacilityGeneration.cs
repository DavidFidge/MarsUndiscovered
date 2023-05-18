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

        if (generationContext.Width % _waveFunctionCollapseGeneratorPasses.TileWidth != 0)
        {
            throw new ArgumentException(
                "Map width must be a multiple of the tile width of the passes - each pixel in the tile width represents one map unit so map width passed in gets divided by the texture tile size to make this happen..",
                nameof(generationContext.Width));
        }

        if (generationContext.Height % _waveFunctionCollapseGeneratorPasses.TileHeight != 0)
        {
            throw new ArgumentException(
                "Map height must be a multiple of the tile height of the passes - each pixel in the tile height represents one map unit so map height passed in gets divided by the texture tile size to make this happen.",
                nameof(generationContext.Height));
        }

        var mapWidth = generationContext.Width / _waveFunctionCollapseGeneratorPasses.TileWidth;
        var mapHeight = generationContext.Height / _waveFunctionCollapseGeneratorPasses.TileHeight;

        _waveFunctionCollapseGeneratorPasses.MapOptions.MapWidth = mapWidth;
        _waveFunctionCollapseGeneratorPasses.MapOptions.MapHeight = mapHeight;
        _waveFunctionCollapseGeneratorPasses.CreatePasses();
        _waveFunctionCollapseGeneratorPasses.ExecuteUntilSuccess();

        var texture2D = _waveFunctionCollapseGeneratorPasses.RenderToTexture2D(_renderer);

        var data = new Color[texture2D.Width * texture2D.Height];

        texture2D.GetData(data);

        var miningFacilityFloorArrayView = new ArrayView<bool>(generationContext.Width, generationContext.Height);
        var arrayView = new ArrayView<GameObjectType>(generationContext.Width, generationContext.Height);

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
                miningFacilityFloorArrayView[index] = true;
            }
            else if (data[index].Equals(Color.White))
            {
                gameObjectType = FloorType.RockFloor;
            }
            else if (data[index].Equals(Color.Red)) 
            {
                gameObjectType = WallType.StockpileWall;
            }
            else
            {
                throw new Exception("Unknown colour in mining facility tiles");
            }
            
            arrayView[index] = gameObjectType;
        }
        
        generationContext.Add(arrayView, MapGenerator.WallFloorTypeTag);
        generationContext.Add(miningFacilityFloorArrayView, MapGenerator.MiningFacilityFloorTag);

        yield return null;
    }
}