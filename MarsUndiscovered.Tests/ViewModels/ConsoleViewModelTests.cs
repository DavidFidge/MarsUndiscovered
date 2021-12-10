using System;
using System.Linq;
using System.Threading;

using MarsUndiscovered.Messages;
using MarsUndiscovered.Messages.Console;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;

using FrigidRogue.MonoGame.Core.ConsoleCommands;
using FrigidRogue.MonoGame.Core.Interfaces.ConsoleCommands;
using FrigidRogue.MonoGame.Core.Messages;
using FrigidRogue.TestInfrastructure;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NGenerics.Extensions;
using NSubstitute;

namespace MarsUndiscovered.Tests.ViewModels
{
    [TestClass]
    public class ConsoleViewModelTests : BaseTest
    {
        private ConsoleViewModel _consoleViewModel;
        private IConsoleCommandServiceFactory _consoleCommandServiceFactory;
        private IConsoleCommand _consoleCommand;

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();

            _consoleCommandServiceFactory = Substitute.For<IConsoleCommandServiceFactory>();

            _consoleCommand = Substitute.For<IConsoleCommand>();

            _consoleViewModel = SetupBaseComponent(new ConsoleViewModel(_consoleCommandServiceFactory));

            _consoleViewModel.Initialize();
        }

        [TestMethod]
        public void Should_Add_To_LastCommands_When_Command_Executes()
        {
            // Arrange
            _consoleCommandServiceFactory
                .CommandFor(Arg.Is<ConsoleCommand>(c => c.Name == "Test"))
                .Returns(_consoleCommand);

            _consoleCommand
                .When(c => c.Execute(Arg.Any<ConsoleCommand>()))
                .Do(ci => ci.Arg<ConsoleCommand>().Result = "ExecuteCalled");

            // Act
            _consoleViewModel.Data.Command = "Test";
            _consoleViewModel.Handle(new ExecuteConsoleCommandRequest(), new CancellationToken());

            // Assert
            Assert.AreEqual(1, _consoleViewModel.Data.LastCommands.Count);

            var command = _consoleViewModel.Data.LastCommands.First;
            Assert.AreEqual("Test", command.Value.Name);
            Assert.AreEqual("ExecuteCalled", command.Value.Result);
        }

        [TestMethod]
        public void Should_Return_CommandNotFound_When_Command_Not_Returned_From_Factory()
        {
            // Arrange
            _consoleCommandServiceFactory
                .CommandFor(Arg.Is<ConsoleCommand>(c => c.Name == "Test"))
                .Returns((IConsoleCommand)null);

            // Act
            _consoleViewModel.Data.Command = "Test";
            _consoleViewModel.Handle(new ExecuteConsoleCommandRequest(), new CancellationToken());

            // Assert
            Assert.AreEqual(1, _consoleViewModel.Data.LastCommands.Count);

            var command = _consoleViewModel.Data.LastCommands.First;
            Assert.AreEqual("Test", command.Value.Name);
            Assert.AreEqual("Command not found", command.Value.Result);
        }

        [TestMethod]
        public void Should_Notify_When_Command_Is_Handled()
        {
            // Arrange
            _consoleCommandServiceFactory
                .CommandFor(Arg.Is<ConsoleCommand>(c => c.Name == "Test"))
                .Returns(_consoleCommand);

            // Act
            _consoleViewModel.Data.Command = "Test";

            _consoleViewModel.Handle(new ExecuteConsoleCommandRequest(), new CancellationToken());

            // Assert
            _consoleViewModel.Mediator
                .Received()
                .Send(Arg.Any<UpdateViewRequest<ConsoleData>>());
        }

        [TestMethod]
        public void Should_Clear_Command_When_Command_Is_Handled()
        {
            // Arrange
            _consoleCommandServiceFactory
                .CommandFor(Arg.Is<ConsoleCommand>(c => c.Name == "Test"))
                .Returns(_consoleCommand);

            // Act
            _consoleViewModel.Data.Command = "Test";

            _consoleViewModel.Handle(new ExecuteConsoleCommandRequest(), new CancellationToken());

            // Assert
            Assert.AreEqual(String.Empty, _consoleViewModel.Data.Command);
        }

        [TestMethod]
        public void Should_Notify_When_Command_Not_Found()
        {
            // Arrange
            _consoleCommandServiceFactory
                .CommandFor(Arg.Is<ConsoleCommand>(c => c.Name == "Test"))
                .Returns((IConsoleCommand)null);

            // Act
            _consoleViewModel.Data.Command = "Test";

            _consoleViewModel.Handle(new ExecuteConsoleCommandRequest(), new CancellationToken());

            // Assert
            _consoleViewModel.Mediator
                .Received()
                .Send(Arg.Any<UpdateViewRequest<ConsoleData>>());
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void Should_Do_Nothing_For_Null_Or_Empty_String(string command)
        {
            // Act
            _consoleViewModel.Data.Command = command;

            _consoleViewModel.Handle(new ExecuteConsoleCommandRequest(), new CancellationToken());

            // Assert
            _consoleViewModel.Mediator
                .DidNotReceive()
                .Send(Arg.Any<UpdateViewRequest<ConsoleData>>());

            Assert.IsTrue(_consoleViewModel.Data.LastCommands.IsEmpty());
        }

        [TestMethod]
        public void Should_Do_Nothing_When_Recall_History_Back_With_No_History()
        {
            // Arrange
            _consoleViewModel.Data.Command = "X";

            // Act
            _consoleViewModel.Handle(new RecallConsoleHistoryBackRequest(), new CancellationToken());

            // Assert
            Assert.AreEqual("X", _consoleViewModel.Data.Command);
        }

        [TestMethod]
        public void Should_Do_Nothing_When_Recall_History_Forward_With_No_History()
        {
            // Arrange
            _consoleViewModel.Data.Command = "X";

            // Act
            _consoleViewModel.Handle(new RecallConsoleHistoryForwardRequest(), new CancellationToken());

            // Assert
            Assert.AreEqual("X", _consoleViewModel.Data.Command);
        }

        [TestMethod]
        public void Should_Recall_Last_Command_When_Recall_History_Back()
        {
            // Arrange
            _consoleViewModel.Data.Command = "History1";

            _consoleViewModel.Handle(new ExecuteConsoleCommandRequest(), new CancellationToken());

            _consoleViewModel.Data.Command = "X";

            // Act
            _consoleViewModel.Handle(new RecallConsoleHistoryBackRequest(), new CancellationToken());

            // Assert
            Assert.AreEqual("History1", _consoleViewModel.Data.Command);
        }

        [TestMethod]
        public void Should_Stay_On_Last_Command_When_Recall_History_Back_Twice_With_One_Item()
        {
            // Arrange
            _consoleViewModel.Data.Command = "History1";

            _consoleViewModel.Handle(new ExecuteConsoleCommandRequest(), new CancellationToken());

            _consoleViewModel.Data.Command = "X";

            // Act
            _consoleViewModel.Handle(new RecallConsoleHistoryBackRequest(), new CancellationToken());
            _consoleViewModel.Handle(new RecallConsoleHistoryBackRequest(), new CancellationToken());

            // Assert
            Assert.AreEqual("History1", _consoleViewModel.Data.Command);
        }

        [TestMethod]
        public void Should_Recall_Earliest_Command_When_Recall_History_Back_Twice()
        {
            // Arrange
            _consoleViewModel.Data.Command = "History1";

            _consoleViewModel.Handle(new ExecuteConsoleCommandRequest(), new CancellationToken());

            _consoleViewModel.Data.Command = "History2";

            _consoleViewModel.Handle(new ExecuteConsoleCommandRequest(), new CancellationToken());

            _consoleViewModel.Data.Command = "X";

            // Act
            _consoleViewModel.Handle(new RecallConsoleHistoryBackRequest(), new CancellationToken());

            _consoleViewModel.Handle(new RecallConsoleHistoryBackRequest(), new CancellationToken());

            // Assert
            Assert.AreEqual("History1", _consoleViewModel.Data.Command);
        }

        [TestMethod]
        public void Should_Recall_First_Command_When_Recall_History_Back_Twice_Forward_Once()
        {
            // Arrange
            _consoleViewModel.Data.Command = "History1";

            _consoleViewModel.Handle(new ExecuteConsoleCommandRequest(), new CancellationToken());

            _consoleViewModel.Data.Command = "History2";

            _consoleViewModel.Handle(new ExecuteConsoleCommandRequest(), new CancellationToken());

            _consoleViewModel.Data.Command = "X";

            // Act
            _consoleViewModel.Handle(new RecallConsoleHistoryBackRequest(), new CancellationToken());
            _consoleViewModel.Handle(new RecallConsoleHistoryBackRequest(), new CancellationToken());
            _consoleViewModel.Handle(new RecallConsoleHistoryForwardRequest(), new CancellationToken());

            // Assert
            Assert.AreEqual("History2", _consoleViewModel.Data.Command);
        }

        [TestMethod]
        public void Should_Clear_Command_When_Recall_History_Back_Then_Forward()
        {
            // Arrange
            _consoleViewModel.Data.Command = "History1";

            _consoleViewModel.Handle(new ExecuteConsoleCommandRequest(), new CancellationToken());

            _consoleViewModel.Data.Command = "X";

            // Act
            _consoleViewModel.Handle(new RecallConsoleHistoryBackRequest(), new CancellationToken());
            _consoleViewModel.Handle(new RecallConsoleHistoryForwardRequest(), new CancellationToken());

            // Assert
            Assert.AreEqual(String.Empty, _consoleViewModel.Data.Command);
        }

        [TestMethod]
        public void Should_Recall_Command_When_Recall_History_Back_Then_Forward_Then_Back()
        {
            // Arrange
            _consoleViewModel.Data.Command = "History1";

            _consoleViewModel.Handle(new ExecuteConsoleCommandRequest(), new CancellationToken());

            _consoleViewModel.Data.Command = "X";

            // Act
            _consoleViewModel.Handle(new RecallConsoleHistoryBackRequest(), new CancellationToken());
            _consoleViewModel.Handle(new RecallConsoleHistoryForwardRequest(), new CancellationToken());
            _consoleViewModel.Handle(new RecallConsoleHistoryBackRequest(), new CancellationToken());

            // Assert
            Assert.AreEqual("History1", _consoleViewModel.Data.Command);
        }

        [TestMethod]
        public void Should_Reset_Recall_Item_When_Command_Is_Executed()
        {
            // Arrange
            _consoleViewModel.Data.Command = "History1";

            _consoleViewModel.Handle(new ExecuteConsoleCommandRequest(), new CancellationToken());

            _consoleViewModel.Data.Command = "History2";

            _consoleViewModel.Handle(new ExecuteConsoleCommandRequest(), new CancellationToken());

            _consoleViewModel.Handle(new RecallConsoleHistoryBackRequest(), new CancellationToken());

            _consoleViewModel.Handle(new ExecuteConsoleCommandRequest(), new CancellationToken());

            // Act and Assert
            _consoleViewModel.Handle(new RecallConsoleHistoryBackRequest(), new CancellationToken());
            Assert.AreEqual("History2", _consoleViewModel.Data.Command);

            _consoleViewModel.Handle(new RecallConsoleHistoryBackRequest(), new CancellationToken());
            Assert.AreEqual("History2", _consoleViewModel.Data.Command);

            _consoleViewModel.Handle(new RecallConsoleHistoryBackRequest(), new CancellationToken());
            Assert.AreEqual("History1", _consoleViewModel.Data.Command);
        }
    }
}