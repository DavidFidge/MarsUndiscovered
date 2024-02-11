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
        var maskChars = new[]
        {
            "########",
            "#.###.##",
            "########",
        };

        var mask = new ArrayView<bool>(maskChars.SelectMany(x => x.Select(y => y == '#')).ToArray(), maskChars[0].Length);
        
        var tunnelCreator = new AStarTunnelCreator(mask, Distance.Chebyshev);

        var map = new ArrayView<bool>(maskChars.SelectMany(x => x.Select(y => y == '.')).ToArray(), maskChars[0].Length);
        
        // Act
        var result = tunnelCreator.CreateTunnel(map, new Point(2, 1), new Point(4, 1));
        
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
        var maskChars = new[]
        {
            "#######",
            "#####.#",
            "#######",
            "#######",
            "#######",
            "#.#####",
            "#######",
        };
        
        var mask = new ArrayView<bool>(maskChars.SelectMany(x => x.Select(y => y == '#')).ToArray(), maskChars[0].Length);

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
        var maskChars = new[]
        {
            "#######",
            "#.#####",
            "#######",
            "#######",
            "#######",
            "#####.#",
            "#######",
        };

        var mask = new ArrayView<bool>(maskChars.SelectMany(x => x.Select(y => y == '#')).ToArray(), maskChars[0].Length);
        
        var tunnelCreator = new AStarTunnelCreator(mask, Distance.Chebyshev);

        var map = new ArrayView<bool>(maskChars.SelectMany(x => x.Select(y => y == '.')).ToArray(), maskChars[0].Length);

        // Act
        var result = tunnelCreator.CreateTunnel(map, new Point(2, 2), new Point(4, 4));
        
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
        var maskChars = new[]
        {
            "#######",
            "#.#####",
            "#######",
            "#######",
            "#######",
            "#####.#",
            "#######",
        };

        var mask = new ArrayView<bool>(maskChars.SelectMany(x => x.Select(y => y == '#')).ToArray(), maskChars[0].Length);
        
        var tunnelCreator = new AStarTunnelCreator(mask, Distance.Chebyshev);

        var map = new ArrayView<bool>(maskChars.SelectMany(x => x.Select(y => y == '.')).ToArray(), maskChars[0].Length);

        // Act
        var result = tunnelCreator.CreateTunnel(map, new Point(4, 4), new Point(2, 2));
        
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
        var maskChars = new[]
        {
            "#######",
            "#####.#",
            "#######",
            "#######",
            "#######",
            "#.#####",
            "#######",
        };

        var mask = new ArrayView<bool>(maskChars.SelectMany(x => x.Select(y => y == '#')).ToArray(), maskChars[0].Length);
        
        var tunnelCreator = new AStarTunnelCreator(mask, Distance.Chebyshev);

        var map = new ArrayView<bool>(maskChars.SelectMany(x => x.Select(y => y == '.')).ToArray(), maskChars[0].Length);

        // Act
        var result = tunnelCreator.CreateTunnel(map, new Point(2, 4), new Point(4, 2));
        
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
        var maskChars = new[]
        {
            "#XXX#XXX#",
            "##.X#X.##",
            "#XXX#XXX#",
            "#########",
        };
        
        var mask = new ArrayView<bool>(maskChars.SelectMany(x => x.Select(y => y == '#')).ToArray(), maskChars[0].Length);

        var tunnelCreator = new AStarTunnelCreator(mask, Distance.Chebyshev);
        
        var map = new ArrayView<bool>(maskChars.SelectMany(x => x.Select(y => y == '.')).ToArray(), maskChars[0].Length);
        
        // Act
        var result = tunnelCreator.CreateTunnel(map, new Point(1, 1), new Point(7, 1));
        
        // Assert
        Assert.AreEqual(16, result.Count);

        var mapResult = GameObjectWriter.WriteArrayView(map);

        // The little bump at 4,2 is a quirk created by the A* algorithm when distance is Distance.Chebyshev.
        // Diagonal distance is equal to horizontal/vertical, so it goes diagonal up then diagonal down,
        // then the double diagonal code fills in the extra horizontal square.  This results
        // in a 'T' with a dead-end single square.
        var expectedResult = new[]
        {
            "#########",
            "...###...",
            ".###.###.",
            ".........",
        };
        
        CollectionAssert.AreEqual(expectedResult, mapResult);
    }
    
    [TestMethod]
    public void Tunnel_Should_Go_Around_Disallowed_Areas_Euclidean()
    {
        // Arrange
        var maskChars = new[]
        {
            "#XXX#XXX#",
            "##.X#X.##",
            "#XXX#XXX#",
            "#########",
        };
        
        var mask = new ArrayView<bool>(maskChars.SelectMany(x => x.Select(y => y == '#')).ToArray(), maskChars[0].Length);

        var tunnelCreator = new AStarTunnelCreator(mask, Distance.Euclidean);
        
        var map = new ArrayView<bool>(maskChars.SelectMany(x => x.Select(y => y == '.')).ToArray(), maskChars[0].Length);
        
        // Act
        var result = tunnelCreator.CreateTunnel(map, new Point(1, 1), new Point(7, 1));
        
        // Assert
        Assert.AreEqual(15, result.Count);

        var mapResult = GameObjectWriter.WriteArrayView(map);
        
        var expectedResult = new[]
        {
            "#########",
            "...###...",
            ".#######.",
            "........."
        };
        
        CollectionAssert.AreEqual(expectedResult, mapResult);
    }
}