using FrigidRogue.MonoGame.Core.ConsoleCommands;
using FrigidRogue.MonoGame.Core.Interfaces.ConsoleCommands;
using FrigidRogue.TestInfrastructure;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MarsUndiscovered.Tests.ViewModels
{
    [TestClass]
    public class ConsoleViewModConsoleCommandServiceFactoryTests : BaseTest
    {
        [TestInitialize]
        public override void Setup()
        {
            base.Setup();
        }

        [TestMethod]
        public void Should_Return_Null_When_Command_Not_Found()
        {
            // Arrange
            var consoleCommandServiceFactory = new ConsoleCommandServiceFactory(new IConsoleCommand[0]);

            // Act
            var command = consoleCommandServiceFactory.CommandFor(new ConsoleCommand("Test"));

            // Assert
            Assert.IsNull(command);
        }

        [TestMethod]
        public void Should_Return_Command_When_Command_Found()
        {
            // Arrange
            var consoleCommand = new TestCommand();

            var consoleCommandServiceFactory = new ConsoleCommandServiceFactory(new IConsoleCommand[] { consoleCommand });

            // Act
            var result = consoleCommandServiceFactory.CommandFor(new ConsoleCommand("MyCommand"));

            // Assert
            Assert.AreEqual(consoleCommand, result);
        }

        [ConsoleCommand(Name = "MyCommand")]
        public class TestCommand : IConsoleCommand
        {
            public void Execute(ConsoleCommand consoleCommand)
            {
            }
        }
    }
}