using FrigidRogue.WaveFunctionCollapse;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Maps;
using NSubstitute;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Tests.Components.GameWorldTests;

[TestClass]
public class AStarTunnelCreatorTests : BaseGameWorldIntegrationTests
{

    [TestInitialize]
    public override void Setup()
    {
        base.Setup();
    }

    [TestMethod]
    public void Should_Create_Tunnel_Between_Two_Points()
    {
        // Arrange
        var tunnelCreator = new AStarTunnelCreator(Distance.Chebyshev);
        
        var map = new[]
        {
            "########",
            "#.###.##",
            "########",
        };
        
        var mask = new ArrayView<bool>(map.SelectMany(x => x.Select(y => y == '#')).ToArray(), map[0].Length);
        
        // Act
        var result = tunnelCreator.CreateTunnel(mask, new Point(2, 1), new Point(4, 1));
        
        // Assert
        Assert.AreEqual(3, result.Count);
        
        Assert.IsTrue(result.Contains(new Point(2, 1)));
        Assert.IsTrue(result.Contains(new Point(3, 1)));
        Assert.IsTrue(result.Contains(new Point(4, 1)));
    }
    
    [TestMethod]
    public void Should_Dig_Double_Sized_DownLeft()
    {
        // Arrange
        var maskLines = new[]
        {
            "#######",
            "#####.#",
            "#######",
            "#######",
            "#######",
            "#.#####",
            "#######",
        };
        
        var mask = new ArrayView<bool>(maskLines.SelectMany(x => x.Select(y => y == '#')).ToArray(), maskLines[0].Length);

        var tunnelCreator = new AStarTunnelCreator(mask, Distance.Chebyshev);

        var map = new ArrayView<bool>(mask.Width, mask.Width);
        
        // Act
        var result = tunnelCreator.CreateTunnel(map, new Point(4, 2), new Point(2, 4));
        
        // Assert
        Assert.AreEqual(5, result.Count);
        
        Assert.IsTrue(result.Contains(new Point(2, 4)));
        Assert.IsTrue(result.Contains(new Point(2, 3)));
        Assert.IsTrue(result.Contains(new Point(3, 3)));
        Assert.IsTrue(result.Contains(new Point(3, 2)));
        Assert.IsTrue(result.Contains(new Point(4, 2)));
    }
    
    [TestMethod]
    public void Should_Dig_Double_Sized_DownRight()
    {
        // Arrange
        var tunnelCreator = new AStarTunnelCreator(Distance.Chebyshev);
        
        var map = new[]
        {
            "#######",
            "#.#####",
            "#######",
            "#######",
            "#######",
            "#####.#",
            "#######",
        };
        
        var mask = new ArrayView<bool>(map.SelectMany(x => x.Select(y => y == '#')).ToArray(), map[0].Length);
        
        // Act
        var result = tunnelCreator.CreateTunnel(mask, new Point(2, 2), new Point(4, 4));
        
        // Assert
        Assert.AreEqual(5, result.Count);
        
        Assert.IsTrue(result.Contains(new Point(2, 2)));
        Assert.IsTrue(result.Contains(new Point(3, 2)));
        Assert.IsTrue(result.Contains(new Point(3, 3)));
        Assert.IsTrue(result.Contains(new Point(4, 3)));
        Assert.IsTrue(result.Contains(new Point(4, 4)));
    }
    
    [TestMethod]
    public void Should_Dig_Double_Sized_UpLeft()
    {
        // Arrange
        var tunnelCreator = new AStarTunnelCreator(Distance.Chebyshev);
        
        var map = new[]
        {
            "#######",
            "#.#####",
            "#######",
            "#######",
            "#######",
            "#####.#",
            "#######",
        };

        var mask = new ArrayView<bool>(map.SelectMany(x => x.Select(y => y == '#')).ToArray(), map[0].Length);
        
        // Act
        var result = tunnelCreator.CreateTunnel(mask, new Point(4, 4), new Point(2, 2));
        
        // Assert
        Assert.AreEqual(5, result.Count);
        
        Assert.IsTrue(result.Contains(new Point(2, 2)));
        Assert.IsTrue(result.Contains(new Point(2, 3)));
        Assert.IsTrue(result.Contains(new Point(3, 3)));
        Assert.IsTrue(result.Contains(new Point(3, 4)));
        Assert.IsTrue(result.Contains(new Point(4, 4)));
    }
    
    [TestMethod]
    public void Should_Dig_Double_Sized_UpRight()
    {
        // Arrange
        var tunnelCreator = new AStarTunnelCreator(Distance.Chebyshev);
        
        var map = new[]
        {
            "#######",
            "#####.#",
            "#######",
            "#######",
            "#######",
            "#.#####",
            "#######",
        };
        
        var mask = new ArrayView<bool>(map.SelectMany(x => x.Select(y => y == '#')).ToArray(), map[0].Length);
        
        // Act
        var result = tunnelCreator.CreateTunnel(mask, new Point(2, 4), new Point(4, 2));
        
        // Assert
        Assert.AreEqual(5, result.Count);
        
        Assert.IsTrue(result.Contains(new Point(2, 4)));
        Assert.IsTrue(result.Contains(new Point(3, 4)));
        Assert.IsTrue(result.Contains(new Point(3, 3)));
        Assert.IsTrue(result.Contains(new Point(4, 3)));
        Assert.IsTrue(result.Contains(new Point(4, 2)));
    }
    
    [TestMethod]
    public void Tunnel_Should_Go_Around_Disallowed_Areas()
    {
        // Arrange
        var tunnelCreator = new AStarTunnelCreator(Distance.Chebyshev);
        
        var map = new[]
        {
            "#XXX#XXX#",
            "##.X#X.##",
            "#XXX#XXX#",
            "#########",
        };
        
        var mask = new ArrayView<bool>(map.SelectMany(x => x.Select(y => y == '#')).ToArray(), map[0].Length);
        
        // Act
        var result = tunnelCreator.CreateTunnel(mask, new Point(1, 1), new Point(7, 1));
        
        // Assert
        Assert.AreEqual(16, result.Count);
        
        Assert.IsTrue(result.Contains(new Point(1, 1)));
        Assert.IsTrue(result.Contains(new Point(0, 1)));
        Assert.IsTrue(result.Contains(new Point(0, 2)));
        Assert.IsTrue(result.Contains(new Point(0, 3)));
        Assert.IsTrue(result.Contains(new Point(1, 3)));
        Assert.IsTrue(result.Contains(new Point(2, 3)));
        Assert.IsTrue(result.Contains(new Point(3, 3)));
        Assert.IsTrue(result.Contains(new Point(4, 3)));
        
        // This is a quirk created by the A* algorithm when distance is Distance.Chebyshev.
        // Diagonal distance is equal to horizontal/vertical, so it goes diagonal up then diagonal down,
        // then the double diagonal code fills in the extra horizontal square.  This results
        // in a 'T' with a dead-end single square.
        Assert.IsTrue(result.Contains(new Point(4, 2)));
        Assert.IsTrue(result.Contains(new Point(5, 3)));
        Assert.IsTrue(result.Contains(new Point(6, 3)));
        Assert.IsTrue(result.Contains(new Point(7, 3)));
        Assert.IsTrue(result.Contains(new Point(8, 3)));
        Assert.IsTrue(result.Contains(new Point(8, 2)));
        Assert.IsTrue(result.Contains(new Point(8, 1)));
        Assert.IsTrue(result.Contains(new Point(7, 1)));
    }
    
    [TestMethod]
    public void Tunnel_Should_Go_Around_Disallowed_Areas_Euclidean()
    {
        // Arrange
        var tunnelCreator = new AStarTunnelCreator(Distance.Euclidean);
        
        var map = new[]
        {
            "#XXX#XXX#",
            "##.X#X.##",
            "#XXX#XXX#",
            "#########",
        };
        
        var mask = new ArrayView<bool>(map.SelectMany(x => x.Select(y => y == '#')).ToArray(), map[0].Length);
        
        // Act
        var result = tunnelCreator.CreateTunnel(mask, new Point(1, 1), new Point(7, 1));
        
        // Assert
        Assert.AreEqual(15, result.Count);
        
        Assert.IsTrue(result.Contains(new Point(1, 1)));
        Assert.IsTrue(result.Contains(new Point(0, 1)));
        Assert.IsTrue(result.Contains(new Point(0, 2)));
        Assert.IsTrue(result.Contains(new Point(0, 3)));
        Assert.IsTrue(result.Contains(new Point(1, 3)));
        Assert.IsTrue(result.Contains(new Point(2, 3)));
        Assert.IsTrue(result.Contains(new Point(3, 3)));
        Assert.IsTrue(result.Contains(new Point(4, 3)));
        Assert.IsTrue(result.Contains(new Point(5, 3)));
        Assert.IsTrue(result.Contains(new Point(6, 3)));
        Assert.IsTrue(result.Contains(new Point(7, 3)));
        Assert.IsTrue(result.Contains(new Point(8, 3)));
        Assert.IsTrue(result.Contains(new Point(8, 2)));
        Assert.IsTrue(result.Contains(new Point(8, 1)));
        Assert.IsTrue(result.Contains(new Point(7, 1)));
    }
}