using FrigidRogue.MonoGame.Core.Extensions;
using FrigidRogue.WaveFunctionCollapse;
using FrigidRogue.WaveFunctionCollapse.Options;
using FrigidRogue.WaveFunctionCollapse.Renderers;

using GoRogue.Random;

using MarsUndiscovered.Game.Components.GenerationSteps;
using MarsUndiscovered.Game.Components.Maps;
using MarsUndiscovered.Game.Extensions;

using Microsoft.Xna.Framework.Graphics;
using NSubstitute;
using SadRogue.Primitives;

using ShaiRandom.Generators;

namespace MarsUndiscovered.Tests.Components.GameWorldTests;

[TestClass]
public class ContiguousWallFinderTests : BaseGameWorldIntegrationTests
{
    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(4)]
    [DataRow(5)]
    [DataRow(6)]
    [DataRow(7)]
    public void Should_Intersect_On_Longest_Path_In_Middle_Of_Map(int situation)
    {
        // Arrange
        var contiguousWallFinder = new ContiguousWallFinder();

        string[] lines;

        if (situation == 1)
        {
            lines = new[]
            {
                "##.",
                "###",
                "###",
                "###",
                "##."
            };
        }
        else if (situation == 2)
        {
            lines = new[]
            {
                ".#.",
                "###",
                "###",
                "###",
                ".#."
            };
        }
        else if (situation == 3)
        {
            lines = new[]
            {
                ".#.",
                "###",
                "###",
                "###",
                ".#."
            };
        }
        else if (situation == 4)
        {
            lines = new[]
            {
                ".##",
                "###",
                "###",
                "###",
                ".##"
            };
        }
        else if (situation == 5)
        {
            lines = new[]
            {
                "...",
                "###",
                "###",
                "###",
                "..."
            };
        }
        else if (situation == 6)
        {
            lines = new[]
            {
                "...",
                "...",
                ".#.",
                "...",
                "..."
            };
        }
        else
        {
            lines = new[]
            {
                "###",
                "###",
                "###",
                "###",
                "###"
            };
        }
        var mapTemplate = new MapTemplate(lines, 0, 0);
        var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, mapTemplate.Where(m => m.Char == '#').Select(m => m.Point).ToList());

        mapGenerator.CreatePrefabMap(_gameWorld, _gameWorld.GameObjectFactory, 3, 5);

        // Act
        contiguousWallFinder.Execute(mapGenerator.Map.WalkabilityView.ToArrayView(v => v), null);

        var intersect = contiguousWallFinder.LongestXYIntersect();

        // Assert
        Assert.AreEqual(new Point(1, 2), intersect);
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    public void Should_Intersect_On_Edge_Of_Map(int situation)
    {
        // Arrange
        var contiguousWallFinder = new ContiguousWallFinder();

        string[] lines;

        if (situation == 1)
        {
            lines = new[]
            {
                "#..",
                "###",
                "###",
                "###",
                "#.."
            };
        }
        else if (situation == 2)
        {
            lines = new[]
            {
                "#.#",
                "#.#",
                "#..",
                "#.#",
                "#.#"
            };
        }
        else
        {
            lines = new[]
            {
                "#..",
                "#..",
                "#..",
                "#..",
                "#.."
            };
        }

        var mapTemplate = new MapTemplate(lines, 0, 0);
        var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, mapTemplate.Where(m => m.Char == '#').Select(m => m.Point).ToList());

        mapGenerator.CreatePrefabMap(_gameWorld, _gameWorld.GameObjectFactory, 3, 5);

        // Act
        contiguousWallFinder.Execute(mapGenerator.Map.WalkabilityView.ToArrayView(v => v), null);

        var intersect = contiguousWallFinder.LongestXYIntersect();

        // Assert
        Assert.AreEqual(new Point(0, 2), intersect);
    }

    [TestMethod]
    public void Should_Not_Intersect()
    {
        // Arrange
        var contiguousWallFinder = new ContiguousWallFinder();

        string[] lines = new[]
            {
                "...",
                "...",
                "...",
                "...",
                "..."
            };
        
        var mapTemplate = new MapTemplate(lines, 0, 0);
        var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, mapTemplate.Where(m => m.Char == '#').Select(m => m.Point).ToList());

        mapGenerator.CreatePrefabMap(_gameWorld, _gameWorld.GameObjectFactory, 3, 5);

        // Act
        contiguousWallFinder.Execute(mapGenerator.Map.WalkabilityView.ToArrayView(v => v), null);

        var intersect = contiguousWallFinder.LongestXYIntersect();

        // Assert
        Assert.AreEqual(Point.None, intersect);
    }
}