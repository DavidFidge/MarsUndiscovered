using FrigidRogue.MonoGame.Core.Graphics.Quads;
using MarsUndiscovered.Graphics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;

namespace MarsUndiscovered.Tests.Graphics;

[TestClass]
public class MapTileGraphicsTests : BaseGraphicsTest
{
    [TestMethod]
    public void Build_Should_Create_TextureAtlas()
    {
        // Arrange
        var mapTileGraphics = new MapTileGraphics();

        var mapTileTexture1 = new MapTileTexture(GraphicsDevice, Constants.TileWidth, Constants.TileHeight, Color.Red, 0);
        var mapTileTexture2 = new MapTileTexture(GraphicsDevice, Constants.TileWidth, Constants.TileHeight, Color.Blue, 0);
        var mapTileTexture3 = new MapTileTexture(GraphicsDevice, Constants.TileWidth, Constants.TileHeight, Color.Green, 0);

        mapTileGraphics.AddMapTileTextures(TileAnimationType.Wall, mapTileTexture1);
        mapTileGraphics.AddMapTileTextures(TileAnimationType.Player, mapTileTexture2);
        mapTileGraphics.AddMapTileTextures(TileAnimationType.Gadget, mapTileTexture3);

        // Act
        mapTileGraphics.Build(GraphicsDevice);

        // Assert
        var expectedBounds = new Rectangle(0, 0, Constants.TileWidth * 2, Constants.TileHeight * 2);

        Assert.AreEqual(expectedBounds, mapTileGraphics.SpriteSheet.TextureAtlas.Texture.Bounds);

        // Wall Tile
        var mapTileTextureResult1 = mapTileGraphics.GetMapTileTexture(TileAnimationType.Wall);
        Assert.IsTrue(mapTileTextureResult1 is SpriteSheetMapTileTexture);
        
        var spriteSheetMapTileTextureResult1 = (SpriteSheetMapTileTexture)mapTileTextureResult1;
        Assert.AreEqual(1, spriteSheetMapTileTextureResult1.SpriteSheetAnimation.KeyFrames.Length);

        var textureColours = new Color[Constants.TileWidth * Constants.TileHeight];

        spriteSheetMapTileTextureResult1.SpriteSheetAnimation.CurrentFrame.Texture.GetData(
            0,
            spriteSheetMapTileTextureResult1.SpriteSheetAnimation.CurrentFrame.Bounds,
            textureColours,
            0,
            Constants.TileWidth * Constants.TileHeight
            );

        Assert.AreEqual(textureColours[0], Color.Red);

        // Player Tile
        var mapTileTextureResult2 = mapTileGraphics.GetMapTileTexture(TileAnimationType.Player);
        Assert.IsTrue(mapTileTextureResult2 is SpriteSheetMapTileTexture);

        var spriteSheetMapTileTextureResult2 = (SpriteSheetMapTileTexture)mapTileTextureResult2;
        Assert.AreEqual(1, spriteSheetMapTileTextureResult2.SpriteSheetAnimation.KeyFrames.Length);

        spriteSheetMapTileTextureResult2.SpriteSheetAnimation.CurrentFrame.Texture.GetData(
            0,
            spriteSheetMapTileTextureResult2.SpriteSheetAnimation.CurrentFrame.Bounds,
            textureColours,
            0,
            Constants.TileWidth * Constants.TileHeight
        );

        Assert.AreEqual(textureColours[0], Color.Blue);

        // Gadget Tile
        var mapTileTextureResult3 = mapTileGraphics.GetMapTileTexture(TileAnimationType.Gadget);
        Assert.IsTrue(mapTileTextureResult3 is SpriteSheetMapTileTexture);

        var spriteSheetMapTileTextureResult3 = (SpriteSheetMapTileTexture)mapTileTextureResult3;
        Assert.AreEqual(1, spriteSheetMapTileTextureResult3.SpriteSheetAnimation.KeyFrames.Length);

        spriteSheetMapTileTextureResult3.SpriteSheetAnimation.CurrentFrame.Texture.GetData(
            0,
            spriteSheetMapTileTextureResult3.SpriteSheetAnimation.CurrentFrame.Bounds,
            textureColours,
            0,
            Constants.TileWidth * Constants.TileHeight
        );

        Assert.AreEqual(textureColours[0], Color.Green);
    }

    [TestMethod]
    public void Build_Should_Create_Two_Frames()
    {
        // Arrange
        var mapTileGraphics = new MapTileGraphics();

        var mapTileTexture1 = new MapTileTexture(GraphicsDevice, Constants.TileWidth, Constants.TileHeight, Color.Red, 0);
        var mapTileTexture2Frame1 = new MapTileTexture(GraphicsDevice, Constants.TileWidth, Constants.TileHeight, Color.Blue, 0);
        var mapTileTexture2Frame2 = new MapTileTexture(GraphicsDevice, Constants.TileWidth, Constants.TileHeight, Color.Yellow, 0);
        var mapTileTexture3 = new MapTileTexture(GraphicsDevice, Constants.TileWidth, Constants.TileHeight, Color.Green, 0);

        mapTileGraphics.AddMapTileTextures(TileAnimationType.Wall, mapTileTexture1);
        mapTileGraphics.AddMapTileTextures(TileAnimationType.Player, mapTileTexture2Frame1, mapTileTexture2Frame2);
        mapTileGraphics.AddMapTileTextures(TileAnimationType.Gadget, mapTileTexture3);

        // Act
        mapTileGraphics.Build(GraphicsDevice);

        // Assert
        var expectedBounds = new Rectangle(0, 0, Constants.TileWidth * 2, Constants.TileHeight * 2);

        Assert.AreEqual(expectedBounds, mapTileGraphics.SpriteSheet.TextureAtlas.Texture.Bounds);

        // Wall Tile
        var mapTileTextureResult1 = mapTileGraphics.GetMapTileTexture(TileAnimationType.Wall);
        Assert.IsTrue(mapTileTextureResult1 is SpriteSheetMapTileTexture);
        
        var spriteSheetMapTileTextureResult1 = (SpriteSheetMapTileTexture)mapTileTextureResult1;
        Assert.AreEqual(1, spriteSheetMapTileTextureResult1.SpriteSheetAnimation.KeyFrames.Length);

        var textureColours = new Color[Constants.TileWidth * Constants.TileHeight];

        spriteSheetMapTileTextureResult1.SpriteSheetAnimation.CurrentFrame.Texture.GetData(
            0,
            spriteSheetMapTileTextureResult1.SpriteSheetAnimation.CurrentFrame.Bounds,
            textureColours,
            0,
            Constants.TileWidth * Constants.TileHeight
            );

        Assert.AreEqual(textureColours[0], Color.Red);

        // Player Tile
        var mapTileTextureResult2 = mapTileGraphics.GetMapTileTexture(TileAnimationType.Player);
        Assert.IsTrue(mapTileTextureResult2 is SpriteSheetMapTileTexture);

        var spriteSheetMapTileTextureResult2 = (SpriteSheetMapTileTexture)mapTileTextureResult2;
        Assert.AreEqual(2, spriteSheetMapTileTextureResult2.SpriteSheetAnimation.KeyFrames.Length);

        // Current frame (same as frame 1)
        spriteSheetMapTileTextureResult2.SpriteSheetAnimation.CurrentFrame.Texture.GetData(
            0,
            spriteSheetMapTileTextureResult2.SpriteSheetAnimation.CurrentFrame.Bounds,
            textureColours,
            0,
            Constants.TileWidth * Constants.TileHeight
        );

        Assert.AreEqual(textureColours[0], Color.Blue);

        // Frame 1
        spriteSheetMapTileTextureResult2.SpriteSheetAnimation.CurrentFrame.Texture.GetData(
            0,
            spriteSheetMapTileTextureResult2.SpriteSheetAnimation.KeyFrames[0].Bounds,
            textureColours,
            0,
            Constants.TileWidth * Constants.TileHeight
        );

        Assert.AreEqual(textureColours[0], Color.Blue);

        // Frame 2
        spriteSheetMapTileTextureResult2.SpriteSheetAnimation.CurrentFrame.Texture.GetData(
            0,
            spriteSheetMapTileTextureResult2.SpriteSheetAnimation.KeyFrames[1].Bounds,
            textureColours,
            0,
            Constants.TileWidth * Constants.TileHeight
        );

        Assert.AreEqual(textureColours[0], Color.Yellow);

        // Gadget Tile
        var mapTileTextureResult3 = mapTileGraphics.GetMapTileTexture(TileAnimationType.Gadget);
        Assert.IsTrue(mapTileTextureResult3 is SpriteSheetMapTileTexture);

        var spriteSheetMapTileTextureResult3 = (SpriteSheetMapTileTexture)mapTileTextureResult3;
        Assert.AreEqual(1, spriteSheetMapTileTextureResult3.SpriteSheetAnimation.KeyFrames.Length);

        spriteSheetMapTileTextureResult3.SpriteSheetAnimation.CurrentFrame.Texture.GetData(
            0,
            spriteSheetMapTileTextureResult3.SpriteSheetAnimation.CurrentFrame.Bounds,
            textureColours,
            0,
            Constants.TileWidth * Constants.TileHeight
        );

        Assert.AreEqual(textureColours[0], Color.Green);
    }

    [TestMethod]
    public void Update_Should_Change_Frames()
    {
        // Arrange
        var mapTileGraphics = new MapTileGraphics();

        var mapTileTextureFrame1 = new MapTileTexture(GraphicsDevice, Constants.TileWidth, Constants.TileHeight, Color.Blue, 0);
        var mapTileTextureFrame2 = new MapTileTexture(GraphicsDevice, Constants.TileWidth, Constants.TileHeight, Color.Yellow, 0);

        mapTileGraphics.AddMapTileTextures(TileAnimationType.Player, mapTileTextureFrame1, mapTileTextureFrame2);

        mapTileGraphics.Build(GraphicsDevice);
        var mapTileTextureResult = mapTileGraphics.GetMapTileTexture(TileAnimationType.Player);
        var spriteSheetMapTileTextureResult = (SpriteSheetMapTileTexture)mapTileTextureResult;

        FakeStopwatchProvider.Elapsed = TimeSpan.FromSeconds(Constants.MapTileAnimationTime);
        FakeGameTimeService.Update(new GameTime());

        // Act
        spriteSheetMapTileTextureResult.Update(FakeGameTimeService);

        // Assert
        var textureColours = new Color[Constants.TileWidth * Constants.TileHeight];

        spriteSheetMapTileTextureResult.SpriteSheetAnimation.CurrentFrame.Texture.GetData(
            0,
            spriteSheetMapTileTextureResult.SpriteSheetAnimation.CurrentFrame.Bounds,
            textureColours,
            0,
            Constants.TileWidth * Constants.TileHeight
        );

        Assert.AreEqual(textureColours[0], Color.Yellow);
    }

    [TestMethod]
    public void Update_Should_Change_Frames_Loop()
    {
        // Arrange
        var mapTileGraphics = new MapTileGraphics();

        var mapTileTextureFrame1 = new MapTileTexture(GraphicsDevice, Constants.TileWidth, Constants.TileHeight, Color.Blue, 0);
        var mapTileTextureFrame2 = new MapTileTexture(GraphicsDevice, Constants.TileWidth, Constants.TileHeight, Color.Yellow, 0);

        mapTileGraphics.AddMapTileTextures(TileAnimationType.Player, mapTileTextureFrame1, mapTileTextureFrame2);

        mapTileGraphics.Build(GraphicsDevice);
        var mapTileTextureResult = mapTileGraphics.GetMapTileTexture(TileAnimationType.Player);
        var spriteSheetMapTileTextureResult = (SpriteSheetMapTileTexture)mapTileTextureResult;

        FakeStopwatchProvider.Elapsed = TimeSpan.FromSeconds(Constants.MapTileAnimationTime);
        FakeGameTimeService.Update(new GameTime());

        spriteSheetMapTileTextureResult.Update(FakeGameTimeService);

        FakeStopwatchProvider.Elapsed = TimeSpan.FromSeconds(Constants.MapTileAnimationTime * 2);
        FakeGameTimeService.Update(new GameTime());

        // Act
        spriteSheetMapTileTextureResult.Update(FakeGameTimeService);

        // Assert
        var textureColours = new Color[Constants.TileWidth * Constants.TileHeight];

        spriteSheetMapTileTextureResult.SpriteSheetAnimation.CurrentFrame.Texture.GetData(
            0,
            spriteSheetMapTileTextureResult.SpriteSheetAnimation.CurrentFrame.Bounds,
            textureColours,
            0,
            Constants.TileWidth * Constants.TileHeight
        );

        Assert.AreEqual(textureColours[0], Color.Blue);
    }
}