using System.Threading.Tasks;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using MarsUndiscovered.Components;
using MarsUndiscovered.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
#pragma warning disable CS4014

namespace MarsUndiscovered.Tests.Components.MorgueTests
{
    [TestClass]
    public class MorgueTests : BaseGameWorldIntegrationTests
    {
        private Morgue _morgue;
        private IMorgueFileWriter _morgueFileWriter;
        private IMorgueWebService _morgueWebService;

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();
            _morgueFileWriter = Substitute.For<IMorgueFileWriter>();
            _morgueWebService = Substitute.For<IMorgueWebService>();
            
            _morgue = new Morgue(_morgueFileWriter, _morgueWebService);
            _morgue.Logger = FakeLogger;
        }

        [TestMethod]
        public async Task SendMorgue_Should_Call_WebService()
        {
            // Arrange
            _gameWorld.NewGame();
            _gameWorld.Player.IsDead = true;
            _morgue.SnapshotMorgueExportData(_gameWorld, "user");
            
            // Act
            await _morgue.SendMorgueToWeb();

            // Assert
            _morgueWebService.Received().SendMorgue(Arg.Any<MorgueExportData>());
        }
        
        [TestMethod]
        public async Task SendMorgue_Should_Log_Warning_If_Morgue_Not_Snapshotted()
        {
            // Act
            await _morgue.SendMorgueToWeb();

            // Assert
            _morgueWebService.DidNotReceive().SendMorgue(Arg.Any<MorgueExportData>());
            Assert.AreEqual("Morgue snapshot is null. Call SnapshotMorgueExportData first.", FakeLogger.LogEvents.Last().MessageTemplate.Text);
        }
        
        [TestMethod]
        public async Task WriteMorgueTextReportToFile_Should_Call_FileWriter()
        {
            // Arrange
            _gameWorld.NewGame();
            _gameWorld.Player.IsDead = true;
            _morgue.SnapshotMorgueExportData(_gameWorld, "TestUser");
            
            // Act
            await _morgue.WriteMorgueToFile();

            // Assert
            _morgueFileWriter.Received().WriteMorgueTextReportToFile(Arg.Any<string>(), Arg.Any<string>(),
                Arg.Any<Guid>());
        }
        
        [TestMethod]
        public async Task WriteMorgueTextReportToFile_Should_Log_Warning_If_Morgue_Not_Snapshotted()
        {
            // Act
            await _morgue.WriteMorgueToFile();

            // Assert
            _morgueFileWriter.DidNotReceive().WriteMorgueTextReportToFile(Arg.Any<string>(), Arg.Any<string>(),
                Arg.Any<Guid>());
            Assert.AreEqual("Morgue snapshot is null. Call SnapshotMorgueExportData first.", FakeLogger.LogEvents.Last().MessageTemplate.Text);
        }
        
        [TestMethod]
        public void SnapshotMorgueExportData_Should_Log_Warning_When_Player_Is_Not_Dead_Or_Victorious()
        {
            // Arrange
            var gameWorld = Substitute.For<IGameWorld>();
            var player = new Player(gameWorld, 1);
            gameWorld.Player.Returns(player);
            
            // Act
            _morgue.SnapshotMorgueExportData(gameWorld, "TestUser");

            // Assert
            Assert.AreEqual("Cannot snapshot morgue data - player is not dead and victory has not been achieved.", FakeLogger.LogEvents.Last().MessageTemplate.Text); 
            
            _morgueFileWriter.DidNotReceive().WriteMorgueTextReportToFile(Arg.Any<string>(), Arg.Any<string>(),
                Arg.Any<Guid>());
        }
        
        [TestMethod]
        public async Task WriteMorgueToFile_Should_Write_Report_To_File_With_Correct_Contents()
        {
            // Arrange
            _gameWorld.NewGame(9999);
            _gameWorld.Morgue.GameStarted();
            
            _gameWorld.Player.IsVictorious = true;

            var fakeDateTimeProvider = Substitute.For<IDateTimeProvider>();

            _morgue.DateTimeProvider = fakeDateTimeProvider;

            fakeDateTimeProvider.UtcNow.Returns(new DateTime(2000, 12, 30, 12, 13, 14));
            _morgue.GameStarted();
            fakeDateTimeProvider.UtcNow.Returns(new DateTime(2001, 1, 2, 1, 2, 3));
            _morgue.GameEnded();

            var morgue =
$@"Mars Undiscovered
Game ID: {_gameWorld.GameId}
Seed: 9999
Username: Username12345!@#$%
Start Date: 2000-12-30 12:13:14 UTC
End Date: 2001-01-02 01:02:03 UTC

You won

STATUS
--------------------------------------------------------------------------------
Health {_gameWorld.Player.Health}/{_gameWorld.Player.MaxHealth}

INVENTORY
--------------------------------------------------------------------------------
No items in inventory

ENEMIES DEFEATED
--------------------------------------------------------------------------------
No enemies were defeated
";
            
            _morgue.SnapshotMorgueExportData(_gameWorld, "Username12345!@#$%");

            // Act
            await _morgue.WriteMorgueToFile();

            // Assert
            var report = (string)_morgueFileWriter.ReceivedCalls().First().GetArguments()[0];

            Assert.AreEqual(morgue, report);
        }
        
        [TestMethod]
        public async Task SendMorgue_Should_Catch_Exception_And_Log()
        {
            // Arrange
            _gameWorld.NewGame();
            _gameWorld.Player.IsDead = true;
            _morgue.SnapshotMorgueExportData(_gameWorld, "user");
            
            _morgueWebService.SendMorgue(Arg.Any<MorgueExportData>()).Throws(new Exception("Test Exception Message"));

            // Act
            await _morgue.SendMorgueToWeb();

            // Assert
            Assert.AreEqual("Error while sending morgue file to web site",FakeLogger.LogEvents.Last().MessageTemplate.Text);
            Assert.AreEqual("Test Exception Message",FakeLogger.LogEvents.Last().Exception.Message);
        }
        
        [TestMethod]
        public async Task WriteMorgueTextReportToFile_Should_Catch_Exception_And_Log()
        {
            // Arrange
            _gameWorld.NewGame();
            _gameWorld.Player.IsDead = true;
            _morgue.SnapshotMorgueExportData(_gameWorld, "user");

            _morgueFileWriter.WriteMorgueTextReportToFile(Arg.Any<string>(), Arg.Any<string>(),
                Arg.Any<Guid>()).Throws(new Exception("Test Exception Message"));
            
            // Act
            await _morgue.WriteMorgueToFile();

            // Assert
            Assert.AreEqual("Error while writing morgue to file",FakeLogger.LogEvents.Last().MessageTemplate.Text);
            Assert.AreEqual("Test Exception Message",FakeLogger.LogEvents.Last().Exception.Message);
        }
    }
}