using FrigidRogue.MonoGame.Core.Interfaces.Graphics;
using GoRogue.Pathing;
using MarsUndiscovered.Interfaces;
using MarsUndiscovered.UserInterface.Animation;
using MarsUndiscovered.UserInterface.ViewModels;
using Microsoft.Xna.Framework;
using NSubstitute;
using Point = SadRogue.Primitives.Point;
using TimeSpan = System.TimeSpan;

namespace MarsUndiscovered.Tests.Graphics;

[TestClass]
public class LaserAnimationsTests : BaseIntegrationTest
{
    public class TestMapViewModel : IMapViewModel
    {
        private IAssets _assets;
        public MapTileEntity MapTileEntity { get; private set; }
        public ISceneGraph SceneGraph { get; }
        public int Width { get; }
        public int Height { get; }
        
        public TestMapViewModel()
        {
            MapTileEntity = new MapTileEntity(new Point(0, 0));
            _assets = Substitute.For<IAssets>();

            MapTileEntity.Assets = _assets;
        }
        
        public void UpdateTile(Point point)
        {
        }

        public void UpdateAllTiles()
        {
        }

        public void ClearAnimationAttackTile(Point point)
        {
        }

        public void ClearAnimationAttackTiles(IEnumerable<Point> points)
        {
        }

        public void AnimateAttackTile(Point point, Action<MapTileEntity> action)
        {
            action.Invoke(MapTileEntity);
        }
    }
    
    [TestMethod]
    public void Opacity_Should_Be_Zero_After_Update_At_Time_Zero()
    {
        // Arrange
        var laserAnimation = new LaserAnimation(new Path(new[] { new Point(0, 0), new Point(1, 1)}));
        var mapViewModel = new TestMapViewModel();

        // Act
        laserAnimation.Update(FakeGameTimeService, mapViewModel);
        
        // Assert
        Assert.AreEqual(0f, mapViewModel.MapTileEntity.Opacity);
    }
    
    [TestMethod]
    public void Opacity_Should_Be_Half_After_250_Milliseconds()
    {
        // Arrange
        var laserAnimation = new LaserAnimation(new Path(new[] { new Point(0, 0), new Point(1, 1)}));
        var mapViewModel = new TestMapViewModel();

        // Act
        FakeStopwatchProvider.Elapsed = TimeSpan.FromMilliseconds(250);
        FakeGameTimeService.Update(new GameTime());

        laserAnimation.Update(FakeGameTimeService, mapViewModel);
        
        // Assert
        Assert.AreEqual(0.5f, mapViewModel.MapTileEntity.Opacity);
        Assert.IsFalse(laserAnimation.IsComplete);
    }
    
    [TestMethod]
    public void Opacity_Should_Be_1_After_1000_Milliseconds()
    {
        // Arrange
        var laserAnimation = new LaserAnimation(new Path(new[] { new Point(0, 0), new Point(1, 1)}));
        var mapViewModel = new TestMapViewModel();

        // Act
        FakeStopwatchProvider.Elapsed = TimeSpan.FromMilliseconds(1000);
        FakeGameTimeService.Update(new GameTime());

        laserAnimation.Update(FakeGameTimeService, mapViewModel);
        
        // Assert
        Assert.AreEqual(1f, mapViewModel.MapTileEntity.Opacity);
        Assert.IsTrue(laserAnimation.IsComplete);
    }
}