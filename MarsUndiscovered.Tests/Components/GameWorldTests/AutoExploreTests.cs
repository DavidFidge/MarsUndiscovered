using Microsoft.VisualStudio.TestTools.UnitTesting;

using SadRogue.Primitives;

namespace MarsUndiscovered.Tests.Components.GameWorldTests
{
    [TestClass]
    public class AutoExploreTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        public void AutoExplore_Should_Move_Towards_Unknown_Space()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItemsNoExits();

            _gameWorld.Player.Position = new Point(0, 0);
            var wallPosition1 = new Point(1, 0);
            var wallPosition2 = new Point(1, 1);
            var wallPosition3 = new Point(1, 2);
            var wallPosition4 = new Point(1, 3);

            _gameWorld.CreateWall(wallPosition1);
            _gameWorld.CreateWall(wallPosition2);
            _gameWorld.CreateWall(wallPosition3);
            _gameWorld.CreateWall(wallPosition4);

            _gameWorld.ResetFieldOfView();

            // Act
            var result = _gameWorld.AutoExploreRequest();

            // Assert
            Assert.AreEqual(new Point(0, 1), _gameWorld.Player.Position);

            Assert.AreEqual(5, result.Path.LengthWithStart);
            Assert.AreEqual(new Point(0, 1), result.Path.GetStepWithStart(0));
            Assert.AreEqual(new Point(0, 2), result.Path.GetStepWithStart(1));
            Assert.AreEqual(new Point(0, 3), result.Path.GetStepWithStart(2));
            Assert.AreEqual(new Point(1, 4), result.Path.GetStepWithStart(3));
            Assert.AreEqual(new Point(2, 4), result.Path.GetStepWithStart(4));
        }
    }
}