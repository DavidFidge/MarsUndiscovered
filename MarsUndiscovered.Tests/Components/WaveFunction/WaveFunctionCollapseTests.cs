using FrigidRogue.MonoGame.Core.Extensions;
using FrigidRogue.TestInfrastructure;
using MarsUndiscovered.Game.Components.WaveFunction;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework.Graphics;
using SadRogue.Primitives;
using Color = Microsoft.Xna.Framework.Color;

namespace MarsUndiscovered.Tests.Components.WaveFunction;

[TestClass]
public class WaveFunctionCollapseTests : BaseGraphicsTest
{
    private Texture2D _floorTexture;
    private Texture2D _lineTexture;
    private Texture2D _cornerTexture;
    private TileAttributes _tileAttributes;

    [TestInitialize]
    public override void Setup()
    {
        base.Setup();

        _floorTexture = new Texture2D(GraphicsDevice, 3, 3);
        _lineTexture = new Texture2D(GraphicsDevice, 3, 3);
        _cornerTexture = new Texture2D(GraphicsDevice, 3, 3);

        // Set color data on floorTexture
        var floorTextureData = new Color[9];
        var lineTextureData = new Color[9];
        var cornerTextureData = new Color[9];

        for (var i = 0; i < 9; i++)
        {
            floorTextureData[i] = Color.White;
            lineTextureData[i] = Color.White;
            cornerTextureData[i] = Color.White;
            
            if (i is 1 or 4 or 7)
                lineTextureData[i] = Color.Black;
            
            if (i is 3 or 4 or 5)
                cornerTextureData[i] = Color.Black;
        }

        _floorTexture.SetData(floorTextureData);
        _lineTexture.SetData(lineTextureData);
        _cornerTexture.SetData(cornerTextureData);

        _tileAttributes = new TileAttributes
        {
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "AAA,AAA,AAA,AAA"
                    }
                },
                {
                    "Line", new TileAttribute
                    {
                        Symmetry = "I",
                        Weight = 1,
                        Adapters = "ABA,AAA,ABA,AAA"
                    }
                },
                {
                    "Corner", new TileAttribute
                    {
                        Symmetry = "L",
                        Weight = 1,
                        Adapters = "AAA,AAA,ABA,ABA"
                    }
                }
            }
        };
    }

    [TestMethod]
    public void L_Shaped_Symmetry_Should_Process_Into_Four_Tiles()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapse();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Corner", _floorTexture }
        };

        // Act
        waveFunctionCollapse.CreateTiles(textures, _tileAttributes);

        // Assert
        var tiles = waveFunctionCollapse.Tiles;

        Assert.AreEqual(4, tiles.Count);

        AssertTile(tiles[0], "AAA,AAA,ABA,ABA", _floorTexture);
        AssertTile(tiles[1], "ABA,AAA,AAA,ABA", _floorTexture, expectedRotation: (float)Math.PI / 4);
        AssertTile(tiles[2], "ABA,ABA,AAA,AAA", _floorTexture, expectedRotation: (float)Math.PI / 2);
        AssertTile(tiles[3], "AAA,ABA,ABA,AAA", _floorTexture, expectedRotation: (float)-Math.PI / 4);
    }

    [TestMethod]
    public void I_Shaped_Symmetry_Should_Process_Into_Two_Tiles()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapse();

        var textures = new Dictionary<string, Texture2D>
            {
                { "Line", _floorTexture }
            };

        // Act
        waveFunctionCollapse.CreateTiles(textures, _tileAttributes);

        // Assert
        var tiles = waveFunctionCollapse.Tiles;

        Assert.AreEqual(2, tiles.Count);

        AssertTile(tiles[0], "ABA,AAA,ABA,AAA", _floorTexture);
        AssertTile(tiles[1], "AAA,ABA,AAA,ABA", _floorTexture, expectedRotation: (float)Math.PI / 4);
    }

    [TestMethod]
    public void X_Shaped_Symmetry_Should_Process_Into_One_Tile()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapse();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture }
        };

        // Act
        waveFunctionCollapse.CreateTiles(textures, _tileAttributes);

        // Assert
        var tiles = waveFunctionCollapse.Tiles;

        Assert.AreEqual(1, tiles.Count);

        AssertTile(tiles[0], "AAA,AAA,AAA,AAA", _floorTexture);
    }

    [TestMethod]
    public void Should_Initialise()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapse();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture },
            { "Line", _lineTexture },
            { "Corner", _cornerTexture },
        };

        waveFunctionCollapse.CreateTiles(textures, _tileAttributes);

        // Act
        waveFunctionCollapse.Initialise(2, 2);

        // Assert
        Assert.AreEqual(4, waveFunctionCollapse.CurrentState.Length);
        
        foreach (var tile in waveFunctionCollapse.CurrentState)
        {
            AssertTileResult(
                tile,
                null,
                tile.Point
                    .Neighbours(1, 1)
                    .Select(p => waveFunctionCollapse.CurrentState.First(t => t.Point == p))
                    .ToArray(),
                int.MaxValue,
                false);
        }
    }

    private void AssertTileResult(TileResult tileResult, Tile expectedTile, TileResult[] expectedNeighbours,
        int expectedEntropy, bool expectedCollapsed)
    {
        Assert.AreEqual(expectedTile, tileResult.Tile);
        CollectionAssert.AreEquivalent(expectedNeighbours, tileResult.Neighbours);
        Assert.AreEqual(expectedEntropy, tileResult.Entropy);
        Assert.AreEqual(expectedCollapsed, tileResult.IsCollapsed);
    }

    private void AssertTile(
        Tile tile,
        string expectedAdapters,
        Texture2D expectedTexture,
        SpriteEffects expectedSpriteEffect = SpriteEffects.None,
        float expectedRotation = 0f)
    {
        var adapters = expectedAdapters.Split(",");

        Assert.AreEqual(adapters[0], tile.Adapters[Direction.Up].Pattern);
        Assert.AreEqual(adapters[1], tile.Adapters[Direction.Right].Pattern);
        Assert.AreEqual(adapters[2], tile.Adapters[Direction.Down].Pattern);
        Assert.AreEqual(adapters[3], tile.Adapters[Direction.Left].Pattern);

        Assert.AreEqual(expectedTexture, tile.Texture);
        Assert.AreEqual(expectedSpriteEffect, tile.SpriteEffects);
        Assert.AreEqual(expectedRotation, tile.Rotation);
    }
}