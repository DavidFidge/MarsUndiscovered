﻿using FrigidRogue.MonoGame.Core.Extensions;
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
                        Adapters = "ABA,CCC,ABA,CCC"
                    }
                },
                {
                    "Corner", new TileAttribute
                    {
                        Symmetry = "^",
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

        AssertTile(tiles[0], "ABA,CCC,ABA,CCC", _floorTexture);
        AssertTile(tiles[1], "CCC,ABA,CCC,ABA", _floorTexture, expectedRotation: (float)Math.PI / 4);
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
        waveFunctionCollapse.Reset(2, 2);

        // Assert
        Assert.AreEqual(4, waveFunctionCollapse.CurrentState.Length);
        
        foreach (var tile in waveFunctionCollapse.CurrentState)
        {
            AssertTileResult(
                tile,
                null,
                tile.Point
                    .Neighbours(1, 1, AdjacencyRule.Types.Cardinals)
                    .Select(p => waveFunctionCollapse.CurrentState.First(t => t.Point == p))
                    .ToArray(),
                int.MaxValue,
                false);
        }
    }

    [TestMethod]
    public void Should_Process_Next_Step()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapse();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture }
        };

        waveFunctionCollapse.CreateTiles(textures, _tileAttributes);
        waveFunctionCollapse.Reset(2, 2);

        // Act
        var result = waveFunctionCollapse.NextStep();

        // Assert
        Assert.AreEqual(4, waveFunctionCollapse.CurrentState.Length);
        Assert.IsFalse(result.IsComplete);
        Assert.IsFalse(result.IsFailed);
        
        var tileResults = waveFunctionCollapse.CurrentState
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(1, tileResults.Count);
        Assert.AreEqual(waveFunctionCollapse.Tiles[0], tileResults[0].TileChoice);

        var otherTiles = waveFunctionCollapse.CurrentState.Except(tileResults).ToList();

        Assert.AreEqual(3, otherTiles.Count);
        Assert.AreEqual(2, otherTiles.Where(t => t.Entropy == Int32.MaxValue - 1).Count());
        Assert.AreEqual(1, otherTiles.Where(t => t.Entropy == Int32.MaxValue).Count());

        CollectionAssert.AreEquivalent(tileResults[0].Neighbours, otherTiles.Where(t => t.Entropy == Int32.MaxValue - 1).ToList());
    }

    [TestMethod]
    public void Should_Process_Two_Steps()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapse();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture }
        };

        waveFunctionCollapse.CreateTiles(textures, _tileAttributes);
        waveFunctionCollapse.Reset(2, 2);

        // Act
        waveFunctionCollapse.NextStep();
        var result = waveFunctionCollapse.NextStep();

        // Assert
        Assert.AreEqual(4, waveFunctionCollapse.CurrentState.Length);
        Assert.IsFalse(result.IsComplete);
        Assert.IsFalse(result.IsFailed);
        
        var tileResults = waveFunctionCollapse.CurrentState
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(2, tileResults.Count);
        Assert.AreEqual(waveFunctionCollapse.Tiles[0], tileResults[0].TileChoice);
        Assert.AreEqual(waveFunctionCollapse.Tiles[0], tileResults[1].TileChoice);

        var otherTiles = waveFunctionCollapse.CurrentState.Except(tileResults).ToList();

        Assert.AreEqual(2, otherTiles.Count);
        Assert.AreEqual(2, otherTiles.Where(t => t.Entropy == Int32.MaxValue - 1).Count());
    }

    [TestMethod]
    public void Should_Process_Three_Steps()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapse();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture }
        };

        waveFunctionCollapse.CreateTiles(textures, _tileAttributes);
        waveFunctionCollapse.Reset(2, 2);

        // Act
        waveFunctionCollapse.NextStep();
        waveFunctionCollapse.NextStep();
        var result = waveFunctionCollapse.NextStep();

        // Assert
        Assert.AreEqual(4, waveFunctionCollapse.CurrentState.Length);
        Assert.IsFalse(result.IsComplete);
        Assert.IsFalse(result.IsFailed);

        var tileResults = waveFunctionCollapse.CurrentState
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(3, tileResults.Count);
        Assert.AreEqual(waveFunctionCollapse.Tiles[0], tileResults[0].TileChoice);
        Assert.AreEqual(waveFunctionCollapse.Tiles[0], tileResults[1].TileChoice);
        Assert.AreEqual(waveFunctionCollapse.Tiles[0], tileResults[2].TileChoice);

        var otherTiles = waveFunctionCollapse.CurrentState.Except(tileResults).ToList();

        Assert.AreEqual(1, otherTiles.Count);
        Assert.AreEqual(1, otherTiles.Where(t => t.Entropy == Int32.MaxValue - 2).Count());
    }

    [TestMethod]
    public void Should_Process_Four_Steps()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapse();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture }
        };

        waveFunctionCollapse.CreateTiles(textures, _tileAttributes);
        waveFunctionCollapse.Reset(2, 2);

        // Act
        waveFunctionCollapse.NextStep();
        waveFunctionCollapse.NextStep();
        waveFunctionCollapse.NextStep();
        var result = waveFunctionCollapse.NextStep();

        // Assert
        Assert.AreEqual(4, waveFunctionCollapse.CurrentState.Length);
        Assert.IsFalse(result.IsComplete);
        Assert.IsFalse(result.IsFailed);

        var tileResults = waveFunctionCollapse.CurrentState
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(4, tileResults.Count);
        Assert.AreEqual(waveFunctionCollapse.Tiles[0], tileResults[0].TileChoice);
        Assert.AreEqual(waveFunctionCollapse.Tiles[0], tileResults[1].TileChoice);
        Assert.AreEqual(waveFunctionCollapse.Tiles[0], tileResults[2].TileChoice);
        Assert.AreEqual(waveFunctionCollapse.Tiles[0], tileResults[3].TileChoice);

        var otherTiles = waveFunctionCollapse.CurrentState.Except(tileResults).ToList();

        Assert.AreEqual(0, otherTiles.Count);
    }

    [TestMethod]
    public void Should_Complete_After_All_Steps_Done()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapse();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture }
        };

        waveFunctionCollapse.CreateTiles(textures, _tileAttributes);
        waveFunctionCollapse.Reset(2, 2);

        // Act
        waveFunctionCollapse.NextStep();
        waveFunctionCollapse.NextStep();
        waveFunctionCollapse.NextStep();
        waveFunctionCollapse.NextStep();
        var result = waveFunctionCollapse.NextStep();

        // Assert
        Assert.IsTrue(result.IsComplete);
    }

    [TestMethod]
    public void Should_Fail_If_No_Valid_Tiles()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapse();
        var failingTexture = new Texture2D(GraphicsDevice, 3, 3);

        var textures = new Dictionary<string, Texture2D>
        {
            { "FailingTexture", failingTexture }
        };

        var tileAttributes = new TileAttributes
        {
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "FailingTexture", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "ABC,DEF,GHI,JKL"
                    }
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, tileAttributes);
        waveFunctionCollapse.Reset(2, 1);

        // Act
        var result1 = waveFunctionCollapse.NextStep();
        var result2 = waveFunctionCollapse.NextStep();

        // Assert
        Assert.IsFalse(result1.IsFailed);
        Assert.IsFalse(result1.IsComplete);

        Assert.IsTrue(result2.IsFailed);
        Assert.IsFalse(result2.IsComplete);
    }

    private void AssertTileResult(TileResult tileResult, TileChoice expectedTileChoice, TileResult[] expectedNeighbours,
        int expectedEntropy, bool expectedCollapsed)
    {
        Assert.AreEqual(expectedTileChoice, tileResult.TileChoice);
        CollectionAssert.AreEquivalent(expectedNeighbours, tileResult.Neighbours);
        Assert.AreEqual(expectedEntropy, tileResult.Entropy);
        Assert.AreEqual(expectedCollapsed, tileResult.IsCollapsed);
    }

    private void AssertTile(
        TileChoice tileChoice,
        string expectedAdapters,
        Texture2D expectedTexture,
        SpriteEffects expectedSpriteEffect = SpriteEffects.None,
        float expectedRotation = 0f)
    {
        var adapters = expectedAdapters.Split(",");

        Assert.AreEqual(adapters[0], tileChoice.Adapters[Direction.Up].Pattern);
        Assert.AreEqual(adapters[1], tileChoice.Adapters[Direction.Right].Pattern);
        Assert.AreEqual(adapters[2], tileChoice.Adapters[Direction.Down].Pattern);
        Assert.AreEqual(adapters[3], tileChoice.Adapters[Direction.Left].Pattern);

        Assert.AreEqual(expectedTexture, tileChoice.Texture);
        Assert.AreEqual(expectedSpriteEffect, tileChoice.SpriteEffects);
        Assert.AreEqual(expectedRotation, tileChoice.Rotation);
    }
}