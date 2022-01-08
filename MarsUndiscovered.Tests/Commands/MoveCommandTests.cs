using System;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.TestInfrastructure;
using GoRogue.Components;
using MarsUndiscovered.Commands;
using MarsUndiscovered.Components;
using MarsUndiscovered.Interfaces;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SadRogue.Primitives;

namespace MarsUndiscovered.Tests.Commands
{
    [TestClass]
    public class MoveCommandTests : BaseTest
    {
        private MoveCommand _moveCommand;

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();

            _moveCommand = SetupBaseComponent(new MoveCommand());

            _moveCommand.SetGameWorld(Substitute.For<IGameWorld>());

            _moveCommand.GameTurnService = Substitute.For<IGameTurnService>();
        }

        [TestMethod]
        public void MoveCommand_Should_Move_GameObject()
        {
            // Arrange
            var testGameObject = new TestGameObject(new Point(1, 1), 1);
            var newPosition = new Point(2, 2);
            _moveCommand.Initialise(testGameObject, new Tuple<Point, Point>(new Point(1, 1), newPosition));

            // Act
             _moveCommand.Execute();

            // Assert
            Assert.AreEqual(testGameObject.Position, newPosition);
        }

        private class TestGameObject : MarsGameObject
        {
            public TestGameObject(Point position, int layer, bool isWalkable = true, bool isTransparent = true, Func<uint> idGenerator = null, IComponentCollection customComponentCollection = null) : base(position, layer, isWalkable, isTransparent, idGenerator, customComponentCollection)
            {
            }

            public TestGameObject(int layer, bool isWalkable = true, bool isTransparent = true, Func<uint> idGenerator = null, IComponentCollection customComponentCollection = null) : base(layer, isWalkable, isTransparent, idGenerator, customComponentCollection)
            {
            }
        }
    }
}