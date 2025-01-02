using FrigidRogue.MonoGame.Core.Graphics.Quads;
using MarsUndiscovered.Graphics;
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

        var mapTileTexture1 = new MapTileTexture(GraphicsDevice, UiConstants.TileWidth, UiConstants.TileHeight, Color.Red);
        var mapTileTexture2 = new MapTileTexture(GraphicsDevice, UiConstants.TileWidth, UiConstants.TileHeight, Color.Blue);
        var mapTileTexture3 = new MapTileTexture(GraphicsDevice, UiConstants.TileWidth, UiConstants.TileHeight, Color.Green);

        mapTileGraphics.AddMapTileTextures(TileGraphicType.Wall.ToString(), mapTileTexture1);
        mapTileGraphics.AddMapTileTextures(TileGraphicType.Player.ToString(), mapTileTexture2);
        mapTileGraphics.AddMapTileTextures(TileGraphicType.Gadget.ToString(), mapTileTexture3);

        // Act
        mapTileGraphics.Build(GraphicsDevice);

        // Assert
        var expectedBounds = new Rectangle(0, 0, UiConstants.TileWidth * 2, UiConstants.TileHeight * 2);

        Assert.AreEqual(expectedBounds, mapTileGraphics.SpriteSheet.TextureAtlas.Texture.Bounds);

        // Wall Tile
        var mapTileTextureResult1 = mapTileGraphics.GetMapTileTexture(TileGraphicType.Wall.ToString());
        Assert.IsTrue(mapTileTextureResult1 is SpriteSheetMapTileTexture);
        
        var spriteSheetMapTileTextureResult1 = (SpriteSheetMapTileTexture)mapTileTextureResult1;
        Assert.AreEqual(1, spriteSheetMapTileTextureResult1.AnimatedSprite.Controller.FrameCount);

        var textureColours = new Color[UiConstants.TileWidth * UiConstants.TileHeight];

        spriteSheetMapTileTextureResult1.AnimatedSprite.TextureRegion.Texture.GetData(
            0,
            spriteSheetMapTileTextureResult1.AnimatedSprite.TextureRegion.Bounds,
            textureColours,
            0,
            UiConstants.TileWidth * UiConstants.TileHeight
            );

        Assert.AreEqual(textureColours[0], Color.Red);

        // Player Tile
        var mapTileTextureResult2 = mapTileGraphics.GetMapTileTexture(TileGraphicType.Player.ToString());
        Assert.IsTrue(mapTileTextureResult2 is SpriteSheetMapTileTexture);

        var spriteSheetMapTileTextureResult2 = (SpriteSheetMapTileTexture)mapTileTextureResult2;
        Assert.AreEqual(1, spriteSheetMapTileTextureResult2.AnimatedSprite.Controller.FrameCount);

        spriteSheetMapTileTextureResult2.AnimatedSprite.TextureRegion.Texture.GetData(
            0,
            spriteSheetMapTileTextureResult2.AnimatedSprite.TextureRegion.Bounds,
            textureColours,
            0,
            UiConstants.TileWidth * UiConstants.TileHeight
        );

        Assert.AreEqual(textureColours[0], Color.Blue);

        // Gadget Tile
        var mapTileTextureResult3 = mapTileGraphics.GetMapTileTexture(TileGraphicType.Gadget.ToString());
        Assert.IsTrue(mapTileTextureResult3 is SpriteSheetMapTileTexture);

        var spriteSheetMapTileTextureResult3 = (SpriteSheetMapTileTexture)mapTileTextureResult3;
        Assert.AreEqual(1, spriteSheetMapTileTextureResult3.AnimatedSprite.Controller.FrameCount);

        spriteSheetMapTileTextureResult3.AnimatedSprite.TextureRegion.Texture.GetData(
            0,
            spriteSheetMapTileTextureResult3.AnimatedSprite.TextureRegion.Bounds,
            textureColours,
            0,
            UiConstants.TileWidth * UiConstants.TileHeight
        );

        Assert.AreEqual(textureColours[0], Color.Green);
    }

    [TestMethod]
    public void Build_Should_Create_Two_Frames()
    {
        // Arrange
        var mapTileGraphics = new MapTileGraphics();

        var mapTileTexture1 = new MapTileTexture(GraphicsDevice, UiConstants.TileWidth, UiConstants.TileHeight, Color.Red);
        var mapTileTexture2Frame1 = new MapTileTexture(GraphicsDevice, UiConstants.TileWidth, UiConstants.TileHeight, Color.Blue);
        var mapTileTexture2Frame2 = new MapTileTexture(GraphicsDevice, UiConstants.TileWidth, UiConstants.TileHeight, Color.Yellow);
        var mapTileTexture3 = new MapTileTexture(GraphicsDevice, UiConstants.TileWidth, UiConstants.TileHeight, Color.Green);

        mapTileGraphics.AddMapTileTextures(TileGraphicType.Wall.ToString(), mapTileTexture1);
        mapTileGraphics.AddMapTileTextures(TileGraphicType.Player.ToString(), mapTileTexture2Frame1, mapTileTexture2Frame2);
        mapTileGraphics.AddMapTileTextures(TileGraphicType.Gadget.ToString(), mapTileTexture3);

        // Act
        mapTileGraphics.Build(GraphicsDevice);

        // Assert
        var expectedBounds = new Rectangle(0, 0, UiConstants.TileWidth * 2, UiConstants.TileHeight * 2);

        Assert.AreEqual(expectedBounds, mapTileGraphics.SpriteSheet.TextureAtlas.Texture.Bounds);

        // Wall Tile
        var mapTileTextureResult1 = mapTileGraphics.GetMapTileTexture(TileGraphicType.Wall.ToString());
        Assert.IsTrue(mapTileTextureResult1 is SpriteSheetMapTileTexture);
        
        var spriteSheetMapTileTextureResult1 = (SpriteSheetMapTileTexture)mapTileTextureResult1;
        Assert.AreEqual(1, spriteSheetMapTileTextureResult1.AnimatedSprite.Controller.FrameCount);

        var textureColours = new Color[UiConstants.TileWidth * UiConstants.TileHeight];

        spriteSheetMapTileTextureResult1.AnimatedSprite.TextureRegion.Texture.GetData(
            0,
            spriteSheetMapTileTextureResult1.AnimatedSprite.TextureRegion.Bounds,
            textureColours,
            0,
            UiConstants.TileWidth * UiConstants.TileHeight
            );

        Assert.AreEqual(textureColours[0], Color.Red);

        // Player Tile
        var mapTileTextureResult2 = mapTileGraphics.GetMapTileTexture(TileGraphicType.Player.ToString());
        Assert.IsTrue(mapTileTextureResult2 is SpriteSheetMapTileTexture);

        var spriteSheetMapTileTextureResult2 = (SpriteSheetMapTileTexture)mapTileTextureResult2;
        Assert.AreEqual(2, spriteSheetMapTileTextureResult2.AnimatedSprite.Controller.FrameCount);

        // Current frame
        spriteSheetMapTileTextureResult2.AnimatedSprite.TextureRegion.Texture.GetData(
            0,
            spriteSheetMapTileTextureResult2.AnimatedSprite.TextureRegion.Bounds,
            textureColours,
            0,
            UiConstants.TileWidth * UiConstants.TileHeight
        );

        Assert.AreEqual(textureColours[0], Color.Blue);

        // Advance to Frame 2
        spriteSheetMapTileTextureResult2.Update(new GameTime(TimeSpan.FromSeconds(0.75f), TimeSpan.FromSeconds(0.75f)));
        
        spriteSheetMapTileTextureResult2.AnimatedSprite.TextureRegion.Texture.GetData(
            0,
            spriteSheetMapTileTextureResult2.AnimatedSprite.TextureRegion.Bounds,
            textureColours,
            0,
            UiConstants.TileWidth * UiConstants.TileHeight
        );

        Assert.AreEqual(textureColours[0], Color.Yellow);

        // Gadget Tile
        var mapTileTextureResult3 = mapTileGraphics.GetMapTileTexture(TileGraphicType.Gadget.ToString());
        Assert.IsTrue(mapTileTextureResult3 is SpriteSheetMapTileTexture);

        var spriteSheetMapTileTextureResult3 = (SpriteSheetMapTileTexture)mapTileTextureResult3;
        Assert.AreEqual(1, spriteSheetMapTileTextureResult3.AnimatedSprite.Controller.FrameCount);

        spriteSheetMapTileTextureResult3.AnimatedSprite.TextureRegion.Texture.GetData(
            0,
            spriteSheetMapTileTextureResult3.AnimatedSprite.TextureRegion.Bounds,
            textureColours,
            0,
            UiConstants.TileWidth * UiConstants.TileHeight
        );

        Assert.AreEqual(textureColours[0], Color.Green);
    }

    [TestMethod]
    public void Update_Should_Change_Frames()
    {
        // Arrange
        var mapTileGraphics = new MapTileGraphics();

        var mapTileTextureFrame1 = new MapTileTexture(GraphicsDevice, UiConstants.TileWidth, UiConstants.TileHeight, Color.Blue);
        var mapTileTextureFrame2 = new MapTileTexture(GraphicsDevice, UiConstants.TileWidth, UiConstants.TileHeight, Color.Yellow);

        mapTileGraphics.AddMapTileTextures(TileGraphicType.Player.ToString(), mapTileTextureFrame1, mapTileTextureFrame2);

        mapTileGraphics.Build(GraphicsDevice);
        var mapTileTextureResult = mapTileGraphics.GetMapTileTexture(TileGraphicType.Player.ToString());
        var spriteSheetMapTileTextureResult = (SpriteSheetMapTileTexture)mapTileTextureResult;

        FakeStopwatchProvider.Elapsed = TimeSpan.FromSeconds(UiConstants.MapTileAnimationSeconds);
        FakeGameTimeService.Update(new GameTime());

        // Act
        spriteSheetMapTileTextureResult.Update(FakeGameTimeService.GameTime);

        // Assert
        var textureColours = new Color[UiConstants.TileWidth * UiConstants.TileHeight];

        spriteSheetMapTileTextureResult.AnimatedSprite.TextureRegion.Texture.GetData(
            0,
            spriteSheetMapTileTextureResult.AnimatedSprite.TextureRegion.Bounds,
            textureColours,
            0,
            UiConstants.TileWidth * UiConstants.TileHeight
        );

        Assert.AreEqual(textureColours[0], Color.Yellow);
    }

    [TestMethod]
    public void Update_Should_Change_Frames_Loop()
    {
        // Arrange
        var mapTileGraphics = new MapTileGraphics();

        var mapTileTextureFrame1 = new MapTileTexture(GraphicsDevice, UiConstants.TileWidth, UiConstants.TileHeight, Color.Blue);
        var mapTileTextureFrame2 = new MapTileTexture(GraphicsDevice, UiConstants.TileWidth, UiConstants.TileHeight, Color.Yellow);

        mapTileGraphics.AddMapTileTextures(TileGraphicType.Player.ToString(), mapTileTextureFrame1, mapTileTextureFrame2);

        mapTileGraphics.Build(GraphicsDevice);
        var mapTileTextureResult = mapTileGraphics.GetMapTileTexture(TileGraphicType.Player.ToString());
        var spriteSheetMapTileTextureResult = (SpriteSheetMapTileTexture)mapTileTextureResult;

        FakeStopwatchProvider.Elapsed = TimeSpan.FromSeconds(UiConstants.MapTileAnimationSeconds);
        FakeGameTimeService.Update(new GameTime());

        spriteSheetMapTileTextureResult.Update(FakeGameTimeService.GameTime);

        FakeStopwatchProvider.Elapsed = TimeSpan.FromSeconds(UiConstants.MapTileAnimationSeconds * 2);
        FakeGameTimeService.Update(new GameTime());

        // Act
        spriteSheetMapTileTextureResult.Update(FakeGameTimeService.GameTime);

        // Assert
        var textureColours = new Color[UiConstants.TileWidth * UiConstants.TileHeight];

        spriteSheetMapTileTextureResult.AnimatedSprite.TextureRegion.Texture.GetData(
            0,
            spriteSheetMapTileTextureResult.AnimatedSprite.TextureRegion.Bounds,
            textureColours,
            0,
            UiConstants.TileWidth * UiConstants.TileHeight
        );

        Assert.AreEqual(textureColours[0], Color.Blue);
    }
}