using System;
using System.Threading;

using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;

using DavidFidge.MonoGame.Core.Interfaces.Services;
using DavidFidge.MonoGame.Core.Messages;
using DavidFidge.MonoGame.Core.Services;
using DavidFidge.TestInfrastructure;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using NSubstitute;

namespace MarsUndiscovered.Tests.ViewModels
{
    [TestClass]
    public class GameSpeedViewModelTests : BaseTest
    {
        private GameSpeedViewModel _gameSpeedViewModel;
        private IGameTimeService _gameTimeService;

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();

            _gameTimeService = Substitute.For<IGameTimeService>();
            _gameSpeedViewModel = SetupBaseComponent(new GameSpeedViewModel(_gameTimeService));
            _gameSpeedViewModel.Initialize();
        }

        [TestMethod]
        public void Handle_GameTimeUpdateNotification_Should_Set_Data_Values_From_GameTimeService()
        {
            // Arrange
            var gameTime = new CustomGameTime
            {
                TotalGameTime = TimeSpan.FromSeconds(1)
            };

            var gameTimeUpdateNotification = new GameTimeUpdateNotification(gameTime);

            _gameTimeService.GameTime.Returns(gameTime);

            // Act
            _gameSpeedViewModel.Handle(gameTimeUpdateNotification, CancellationToken.None);

            // Assert
            Assert.AreEqual(TimeSpan.FromSeconds(1), _gameSpeedViewModel.Data.TotalGameTime);

            _gameSpeedViewModel
                .Mediator
                .Received()
                .Send(Arg.Any<UpdateViewRequest<GameSpeedData>>());
        }

        [TestMethod]
        public void Handle_ChangeGameSpeedRequest_PauseGame_Should_Pause()
        {
            // Arrange
            var changeGameSpeedRequest = new ChangeGameSpeedRequest().TogglePauseGameRequest();

            _gameTimeService.When(p => p.PauseGame()).Do(ci => _gameTimeService.IsPaused.Returns(true));

            // Act
            _gameSpeedViewModel.Handle(changeGameSpeedRequest, CancellationToken.None);

            // Assert
            Assert.IsTrue(_gameSpeedViewModel.Data.IsPaused);

            _gameSpeedViewModel
                .Mediator
                .Received()
                .Send(Arg.Any<UpdateViewRequest<GameSpeedData>>());
        }

        [TestMethod]
        public void Handle_ChangeGameSpeedRequest_PauseGame_Should_Resume_If_Game_Already_Paused()
        {
            // Arrange
            var changeGameSpeedRequest = new ChangeGameSpeedRequest().TogglePauseGameRequest();

            _gameTimeService.IsPaused.Returns(true);

            _gameTimeService.When(p => p.ResumeGame()).Do(ci => _gameTimeService.IsPaused.Returns(false));

            // Act
            _gameSpeedViewModel.Handle(changeGameSpeedRequest, CancellationToken.None);

            // Assert
            Assert.IsFalse(_gameSpeedViewModel.Data.IsPaused);

            _gameSpeedViewModel
                .Mediator
                .Received()
                .Send(Arg.Any<UpdateViewRequest<GameSpeedData>>());
        }

        [TestMethod]
        public void Handle_ChangeGameSpeedRequest_IncreaseGameSpeed_Should_Increase_Speed()
        {
            // Arrange
            var changeGameSpeedRequest = new ChangeGameSpeedRequest().IncreaseSpeedRequest();

            _gameTimeService.GameSpeedPercent.Returns(100);

            _gameTimeService.When(p => p.IncreaseGameSpeed()).Do(ci => _gameTimeService.GameSpeedPercent.Returns(200));

            // Act
            _gameSpeedViewModel.Handle(changeGameSpeedRequest, CancellationToken.None);

            // Assert
            Assert.AreEqual(200, _gameSpeedViewModel.Data.GameSpeedPercent);

            _gameSpeedViewModel
                .Mediator
                .Received()
                .Send(Arg.Any<UpdateViewRequest<GameSpeedData>>());
        }

        [TestMethod]
        public void Handle_ChangeGameSpeedRequest_DecreaseGameSpeed_Should_Decrease_Speed()
        {
            // Arrange
            var changeGameSpeedRequest = new ChangeGameSpeedRequest().DecreaseSpeedRequest();

            _gameTimeService.GameSpeedPercent.Returns(100);

            _gameTimeService.When(p => p.DecreaseGameSpeed()).Do(ci => _gameTimeService.GameSpeedPercent.Returns(50));

            // Act
            _gameSpeedViewModel.Handle(changeGameSpeedRequest, CancellationToken.None);

            // Assert
            Assert.AreEqual(50, _gameSpeedViewModel.Data.GameSpeedPercent);

            _gameSpeedViewModel
                .Mediator
                .Received()
                .Send(Arg.Any<UpdateViewRequest<GameSpeedData>>());
        }
    }
}