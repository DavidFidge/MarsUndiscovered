using System.Threading.Tasks;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Serilog.Events;

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
        public async Task SendPendingMorgues_Should_Call_WebService()
        {
            // Arrange
            var morgueExportDatas = new List<MorgueExportData>
            {
                new(),
                new()
            };

            _morgueFileWriter.ReadPendingMorgues().Returns(morgueExportDatas);
            
            // Act
            await _morgue.SendPendingMorgues();

            // Assert
            var calls = _morgueWebService.ReceivedCalls().ToList();
            Assert.AreEqual(2, calls.Count);

            Assert.AreEqual("SendMorgue", calls.First().GetMethodInfo().Name);
            Assert.AreSame(morgueExportDatas[0], calls.First().GetArguments()[0]);

            Assert.AreEqual("SendMorgue", calls.Last().GetMethodInfo().Name);
            Assert.AreSame(morgueExportDatas[1], calls.Last().GetArguments()[0]);

            _morgueFileWriter.Received().DeletePendingMorgue(Arg.Is(morgueExportDatas[0]));
            _morgueFileWriter.Received().DeletePendingMorgue(Arg.Is(morgueExportDatas[1]));

        }

        [TestMethod]
        public async Task SendPendingMorgue_Failure_With_Read_Should_Log_Error()
        {
            // Arrange
            var exception = new Exception();
            _morgueFileWriter.ReadPendingMorgues().Throws(exception);

            // Act
            await _morgue.SendPendingMorgues();

            // Assert
            FakeLogger.AssertLogEvent("Error while sending pending morgues", exception, LogEventLevel.Warning);
            _morgueFileWriter.DidNotReceive().DeletePendingMorgue(Arg.Any<MorgueExportData>());
        }

        [TestMethod]
        public void SnapshotMorgueExportData_Should_Call_MorgueFileWriterWriteMorgueTextReportToFile()
        {
            // Arrange
            _gameWorld.NewGame();
            _gameWorld.Player.IsDead = true;

            // Act
            _morgue.SnapshotMorgueExportData(_gameWorld, "TestUser", true);

            // Assert
            _morgueFileWriter.Received().WriteMorgueTextReportToFile(Arg.Any<string>(), Arg.Any<string>(),
                Arg.Any<Guid>());
        }

        [TestMethod]
        public void SnapshotMorgueExportData_Should_Call_MorgueFileWriterWritePendingMorgue()
        {
            // Arrange
            _gameWorld.NewGame();
            _gameWorld.Player.IsDead = true;

            // Act
            _morgue.SnapshotMorgueExportData(_gameWorld, "TestUser", true);

            // Assert
            _morgueFileWriter.Received().WritePendingMorgue(Arg.Any<MorgueExportData>());
        }

        [TestMethod]
        public void SnapshotMorgueExportData_Should_Not_Call_MorgueFileWriterWritePendingMorgue_When_UploadMorgueFiles_Is_False()
        {
            // Arrange
            _gameWorld.NewGame();
            _gameWorld.Player.IsDead = true;

            // Act
            _morgue.SnapshotMorgueExportData(_gameWorld, "TestUser", false);

            // Assert
            _morgueFileWriter.DidNotReceive().WritePendingMorgue(Arg.Any<MorgueExportData>());
        }

        [TestMethod]
        public void SnapshotMorgueExportData_Should_Log_Warning_When_Player_Is_Not_Dead_Or_Victorious()
        {
            // Arrange
            var gameWorld = Substitute.For<IGameWorld>();
            var player = new Player(gameWorld, 1);
            gameWorld.Player.Returns(player);
            
            // Act
            _morgue.SnapshotMorgueExportData(gameWorld, "TestUser", true);

            // Assert
            Assert.AreEqual("Cannot snapshot morgue data - player is not dead and victory has not been achieved.", FakeLogger.LogEvents.Last().MessageTemplate.Text); 
            
            _morgueFileWriter.DidNotReceive().WriteMorgueTextReportToFile(Arg.Any<string>(), Arg.Any<string>(),
                Arg.Any<Guid>());
        }

        [TestMethod]
        public void SnapshotMorgueExportData_Should_Log_Warning_If_Unable_To_Write_Pending_File()
        {
            // Arrange
            _gameWorld.NewGame();
            _gameWorld.Player.IsDead = true;

            _morgueFileWriter
                .WhenForAnyArgs(c => c.WritePendingMorgue(Arg.Any<MorgueExportData>()))
                .Do(c => { throw new Exception(); });

            // Act
            _morgue.SnapshotMorgueExportData(_gameWorld, "TestUser", true);

            // Assert
            FakeLogger.AssertLogEvent("Error while writing pending morgue to file. Morgue will not be sent to web service.", LogEventLevel.Warning);
        }

        [TestMethod]
        public void WriteMorgueToFile_Should_Write_Report_To_File_With_Correct_Contents()
        {
            // Arrange
            _gameWorld.NewGame(9999);
            
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
Game Version: 0.1.4
Seed: 9999
Username: Username12345!@#$%
Start Date: 2000-12-30 12:13:14 UTC
End Date: 2001-01-02 01:02:03 UTC

Won: You retrieved ship parts

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

            // Act
            _morgue.SnapshotMorgueExportData(_gameWorld, "Username12345!@#$%", true);

            // Assert
            var report = (string)_morgueFileWriter.ReceivedCalls().First().GetArguments()[0];

            Assert.AreEqual(morgue, report);
        }

        [TestMethod]
        public void WriteMorgueToFile_Should_Write_Report_To_File_With_Correct_Contents_After_Reset_And_New_Game()
        {
            // Arrange
            _gameWorld.NewGame(9999);
            
            _gameWorld.Player.IsVictorious = true;

            var fakeDateTimeProvider = Substitute.For<IDateTimeProvider>();

            _morgue.DateTimeProvider = fakeDateTimeProvider;

            _morgue.GameStarted();
            _morgue.GameEnded();
            
            _morgue.SnapshotMorgueExportData(_gameWorld, String.Empty, true);
            
            _gameWorld.NewGame(10000);
            
            _gameWorld.Player.IsDead = true;
            _gameWorld.Player.IsDeadMessage = "killed by a creature";

            fakeDateTimeProvider.UtcNow.Returns(new DateTime(2000, 12, 30, 12, 13, 14));
            _morgue.GameStarted();
            fakeDateTimeProvider.UtcNow.Returns(new DateTime(2001, 1, 2, 1, 2, 3));

            _morgue.GameEnded();

            var morgue =
                $@"Mars Undiscovered
Game ID: {_gameWorld.GameId}
Game Version: 0.1.4
Seed: 10000
Username: Username12345!@#$%
Start Date: 2000-12-30 12:13:14 UTC
End Date: 2001-01-02 01:02:03 UTC

Died: You were killed by a creature

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
            _morgueFileWriter.ClearReceivedCalls();

            // Act
            _morgue.SnapshotMorgueExportData(_gameWorld, "Username12345!@#$%", true);

            // Assert
            var report = (string)_morgueFileWriter.ReceivedCalls().First().GetArguments()[0];

            Assert.AreEqual(morgue, report);
        }

        [TestMethod]
        public async Task SendMorgue_Should_Catch_Exception_And_Log()
        {
            // Arrange
            _morgueFileWriter
                .ReadPendingMorgues()
                .Returns(new List<MorgueExportData> { new() });

            var exception = new Exception("Test Exception Message");
            _morgueWebService.SendMorgue(Arg.Any<MorgueExportData>()).Throws(exception);

            // Act
            await _morgue.SendPendingMorgues();

            // Assert
            FakeLogger.AssertLogEvent("Error while sending morgue file to web site.", exception, LogEventLevel.Warning);
            _morgueFileWriter.DidNotReceive().DeletePendingMorgue(Arg.Any<MorgueExportData>());
        }

        [TestMethod]
        public async Task SendMorgue_Should_Catch_Exception_On_Attempt_To_Delete_File_And_Log()
        {
            // Arrange
            _morgueFileWriter
                .ReadPendingMorgues()
                .Returns(new List<MorgueExportData> { new() });

            var exception = new Exception();
            _morgueFileWriter.WhenForAnyArgs(c => c.DeletePendingMorgue(Arg.Any<MorgueExportData>())).Do(c => throw exception);

            // Act
            await _morgue.SendPendingMorgues();

            // Assert
            FakeLogger.AssertLogEvent("Unable to delete pending morgue file.", exception, LogEventLevel.Warning);
        }

        [TestMethod]
        public void WriteMorgueTextReportToFile_Should_Catch_Exception_And_Log()
        {
            // Arrange
            _gameWorld.NewGame();
            _gameWorld.Player.IsDead = true;
            _morgue.SnapshotMorgueExportData(_gameWorld, "user", true);

            var exception = new Exception("Test Exception Message");
            _morgueFileWriter
                .WhenForAnyArgs(c => c.WriteMorgueTextReportToFile(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Guid>()))
                .Do(c => throw exception);

            // Act
            _morgue.SnapshotMorgueExportData(_gameWorld, "user", true);

            // Assert
            FakeLogger.AssertLogEvent("Error while writing morgue report to file", exception, LogEventLevel.Warning);
        }
    }
}