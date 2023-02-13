using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using FrigidRogue.TestInfrastructure;
using MarsUndiscovered.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MarsUndiscovered.Tests.Components.MorgueTests;

[TestClass]
public class MorgueFileWriterTests : BaseTest
{
    private MorgueFileWriter _morgueFileWriter;

    [TestInitialize]
    public override void Setup()
    {
        _morgueFileWriter = new MorgueFileWriter();
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
    public async Task Should_Write_Morgue_Contents_To_File()
    {
        // Arrange
        var username = "TestUser";
        var gameId = Guid.NewGuid();
        var testMorgueContents = "Test Morgue Contents";

        // Act
        await _morgueFileWriter.WriteMorgueTextReportToFile(testMorgueContents, username, gameId);

        // Assert
        var morgueFilePath = GetMorgueFilePath();
        var file = new FileInfo(Path.Combine(morgueFilePath, $"{username} {gameId}.txt"));
        Assert.IsTrue(file.Exists);

        using var reader = file.OpenText();

        var line = reader.ReadLine();
        
        Assert.AreEqual(testMorgueContents, line);
    }

    private string GetMorgueFilePath()
    {
        var localFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var gameFolder = Path.Combine(localFolderPath, Assembly.GetEntryAssembly().GetName().Name);

        var morgueFilePath = Path.Combine(gameFolder, "Morgue Files");

        return morgueFilePath;
    }
}