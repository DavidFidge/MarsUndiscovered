using System.IO;
using System.Reflection;
using FrigidRogue.TestInfrastructure;
using MarsUndiscovered.Game.Components;
using Serilog.Events;

namespace MarsUndiscovered.Tests.Components.MorgueTests;

[TestClass]
public class MorgueFileWriterTests : BaseTest
{
    private MorgueFileWriter _morgueFileWriter;

    [TestInitialize]
    public override void Setup()
    {
        _morgueFileWriter = new MorgueFileWriter();
        SetupBaseComponent(_morgueFileWriter);
    }

    [TestCleanup]
    public override void TearDown()
    {
        base.TearDown();

        try
        {
            var localFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var gameFolder = Path.Combine(localFolderPath, Assembly.GetEntryAssembly().GetName().Name);

            var morgueFilePath = Path.Combine(gameFolder, "Morgue Files");

            if (Directory.Exists(morgueFilePath))
                Directory.Delete(morgueFilePath, true);

            if (Directory.Exists(gameFolder))
                Directory.Delete(gameFolder, true);
        }
        catch (Exception)
        {
            // ignored
        }
    }

    [TestMethod]
    public void Should_Write_Morgue_Contents_To_File()
    {
        // Arrange
        var username = "TestUser";
        var gameId = Guid.NewGuid();
        var testMorgueContents = "Test Morgue Contents";

        // Act
        _morgueFileWriter.WriteMorgueTextReportToFile(testMorgueContents, username, gameId);

        // Assert
        var morgueFilePath = GetMorgueFilePath();
        var file = new FileInfo(Path.Combine(morgueFilePath, $"{username} {gameId} Morgue.txt"));
        Assert.IsTrue(file.Exists);

        using var reader = file.OpenText();

        var line = reader.ReadLine();

        Assert.AreEqual(testMorgueContents, line);

        reader.Close();
        file.Delete();
    }

    [TestMethod]
    public void Should_Write_Pending_Morgue_File()
    {
        // Arrange
        var morgueExportData = new MorgueExportData();
        morgueExportData.Id = Guid.NewGuid();

        // Act
        _morgueFileWriter.WritePendingMorgue(morgueExportData);

        // Assert
        var morgueFilePath = GetMorgueFilePendingPath();
        var file = new FileInfo(Path.Combine(morgueFilePath, $"Pending{morgueExportData.Id}.txt"));
        Assert.IsTrue(file.Exists);

        file.Delete();
    }

    [TestMethod]
    public void Should_Read_Pending_Morgue_Files()
    {
        // Arrange
        var morgueExportData1 = new MorgueExportData();
        morgueExportData1.Id = Guid.NewGuid();
        morgueExportData1.Username = "Test1";

        var morgueExportData2 = new MorgueExportData();
        morgueExportData2.Id = Guid.NewGuid();
        morgueExportData2.Username = "Test2";

        _morgueFileWriter.WritePendingMorgue(morgueExportData1);
        _morgueFileWriter.WritePendingMorgue(morgueExportData2);

        // Act
        var morgueExportDataItems = _morgueFileWriter.ReadPendingMorgues();

        // Assert
        var result1 = morgueExportDataItems.First(r => r.Id == morgueExportData1.Id);
        var result2 = morgueExportDataItems.First(r => r.Id == morgueExportData2.Id);

        Assert.AreEqual(result1.Id, morgueExportData1.Id);
        Assert.AreEqual(result1.Username, morgueExportData1.Username);

        Assert.AreEqual(result2.Id, morgueExportData2.Id);
        Assert.AreEqual(result2.Username, morgueExportData2.Username);

        var morgueFilePath = GetMorgueFilePendingPath();

        var file1 = new FileInfo(Path.Combine(morgueFilePath, $"Pending{morgueExportData1.Id}.txt"));
        var file2 = new FileInfo(Path.Combine(morgueFilePath, $"Pending{morgueExportData2.Id}.txt"));

        Assert.IsTrue(file1.Exists);
        Assert.IsTrue(file2.Exists);

        file1.Delete();
        file2.Delete();
    }

    [TestMethod]
    public void Should_Rename_Unreadable_File()
    {
        // Arrange
        var guid = Guid.NewGuid();

        var pendingMorgueFile = Path.Combine(GetMorgueFilePendingPath(), $"Pending{guid}.txt");

        using var stream = File.CreateText(pendingMorgueFile);

        stream.WriteLine("Bad Data");

        stream.Close();

        // Act
        var morgueExportDataItems = _morgueFileWriter.ReadPendingMorgues();

        // Assert
        Assert.AreEqual(0, morgueExportDataItems.Count);
        var result = new FileInfo(Path.Combine(GetMorgueFileErrorPath(), $"Error{guid}.txt"));
        Assert.IsTrue(result.Exists);

        FakeLogger.AssertLogEvent($"Could not read pending morgue file Pending{guid}.txt. Renaming to Error{guid}.txt.",
            LogEventLevel.Warning);

        result.Delete();
    }

    [TestMethod]
    public void Should_Read_Pending_Morgue_Files_When_Unreadable_Files_Exist()
    {
        // Arrange
        var morgueExportData = new MorgueExportData();
        morgueExportData.Id = Guid.NewGuid();
        morgueExportData.Username = "Test1";

        var guid = Guid.NewGuid();

        var pendingMorgueFile = Path.Combine(GetMorgueFilePendingPath(), $"Pending{guid}.txt");

        using var stream = File.CreateText(pendingMorgueFile);

        stream.WriteLine("Bad Data");

        stream.Close();

        _morgueFileWriter.WritePendingMorgue(morgueExportData);

        // Act
        var morgueExportDataItems = _morgueFileWriter.ReadPendingMorgues();

        // Assert
        var result1 = morgueExportDataItems.First(r => r.Id == morgueExportData.Id);

        Assert.AreEqual(result1.Id, morgueExportData.Id);
        Assert.AreEqual(result1.Username, morgueExportData.Username);

        var deletedFile = new FileInfo(Path.Combine(GetMorgueFilePendingPath(), $"Pending{guid}.txt"));
        Assert.IsFalse(deletedFile.Exists);

        var errorFile = new FileInfo(Path.Combine(GetMorgueFileErrorPath(), $"Error{guid}.txt"));
        Assert.IsTrue(errorFile.Exists);

        var morgueExportDataFile = new FileInfo(Path.Combine(GetMorgueFilePendingPath(), $"Pending{morgueExportData.Id}.txt"));
        Assert.IsTrue(morgueExportDataFile.Exists);

        errorFile.Delete();
        morgueExportDataFile.Delete();
    }

    private string GetMorgueFilePath()
    {
        var localFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var gameFolder = Path.Combine(localFolderPath, Assembly.GetEntryAssembly().GetName().Name);

        var morgueFilePath = Path.Combine(gameFolder, "Morgue Files");

        if (!Directory.Exists(morgueFilePath))
            Directory.CreateDirectory(morgueFilePath);

        return morgueFilePath;
    }

    private string GetMorgueFilePendingPath()
    {
        var morgueFilePath = GetMorgueFilePath();
        morgueFilePath = Path.Combine(morgueFilePath, "Pending");

        if (!Directory.Exists(morgueFilePath))
            Directory.CreateDirectory(morgueFilePath);

        return morgueFilePath;
    }

    private string GetMorgueFileErrorPath()
    {
        var morgueFilePath = GetMorgueFilePath();
        morgueFilePath = Path.Combine(morgueFilePath, "Error");

        if (!Directory.Exists(morgueFilePath))
            Directory.CreateDirectory(morgueFilePath);

        return morgueFilePath;
    }
}