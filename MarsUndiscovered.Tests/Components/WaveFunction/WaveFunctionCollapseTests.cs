using FrigidRogue.TestInfrastructure;
using MarsUndiscovered.Game.Components.WaveFunction;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace MarsUndiscovered.Tests.Components.WaveFunction;

[TestClass]
public class WaveFunctionCollapseTests : BaseGraphicsTest
{
    [TestInitialize]
    public override void Setup()
    {
        var floorTexture = new Texture2D(GraphicsDevice, 3, 3);
        var lineTexture = new Texture2D(GraphicsDevice, 3, 3);
        var cornerTexture = new Texture2D(GraphicsDevice, 3, 3);
        
        // Set color data on floorTexture
        var floorTextureData = new Color[9];
        var lineTextureData = new Color[9];
        var cornerTextureData = new Color[9];
        
        for (var i = 0; i < 9; i++)
        {
            floorTextureData[i] = Color.White;
            lineTextureData[i] = Color.White;
            cornerTextureData[i] = Color.White;
            
            if (i == 1 || i == 4 || i == 7)
                lineTextureData[i] = Color.Black;
            
            if (i == 3 || i == 4 || i == 5)
                cornerTextureData[i] = Color.Black;
        }
        
        floorTexture.SetData(floorTextureData);
        lineTexture.SetData(lineTextureData);
        cornerTexture.SetData(cornerTextureData);

        base.Setup();
    }

    [TestMethod]
    public void TestMethod1()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapse();

        var textures = new Dictionary<string, Texture2D>();
        
        
        var tileAttributes = new TileAttributes();
        
        // Act
        waveFunctionCollapse.CreateTiles(textures, tileAttributes);

        
        // Assert
    }
}