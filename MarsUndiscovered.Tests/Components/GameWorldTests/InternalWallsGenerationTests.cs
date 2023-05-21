using GoRogue.MapGeneration;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.GenerationSteps;
using NSubstitute;
using SadRogue.Primitives;
using ShaiRandom.Generators;

namespace MarsUndiscovered.Tests.Components.GameWorldTests;

[TestClass]
public class InternalWallsGenerationTests : BaseTest
{
    public void Should()
    {
        // Arrange
        var internalWallsGeneration = new InternalWallsGeneration(WallType.RockWall);
        var generationContext = new GenerationContext(10, 10);

        var random = Substitute.For<IEnhancedRandom>();

        random
            .NextInt(Arg.Any<int>(), Arg.Any<int>())
            .Returns(5);

        random.NextInt(Arg.Any<int>()).Returns(1);
        
        // Act
        internalWallsGeneration.PerformStep(generationContext);

        // Assert
        var results = generationContext.GetFirst<AreaWallsDoors>();
        
        
    }
}