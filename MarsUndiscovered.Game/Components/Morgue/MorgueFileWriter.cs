using System.IO;
using System.Reflection;
using FrigidRogue.MonoGame.Core.Components;
using Newtonsoft.Json;

namespace MarsUndiscovered.Game.Components;

public class MorgueFileWriter : BaseComponent, IMorgueFileWriter
{
    private string GetMorgueFilePath()
    {
        var localFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var gameFolder = Path.Combine(localFolderPath, Assembly.GetEntryAssembly().GetName().Name);

        if (!Directory.Exists(gameFolder))
            Directory.CreateDirectory(gameFolder);

        var morgueFilePath = Path.Combine(gameFolder, "Morgue Files");

        if (!Directory.Exists(morgueFilePath))
            Directory.CreateDirectory(morgueFilePath);

        return morgueFilePath;
    }

    public void WriteMorgueTextReportToFile(string morgueTextReport, string username, Guid gameId)
    {
        var morgueFilePath = GetMorgueFilePath();

        var file = new FileInfo(Path.Combine(morgueFilePath, $"{username} {gameId}.txt"));

        using var writer = file.CreateText();

        writer.Write(morgueTextReport);

        writer.Close();
    }

    public void WritePendingMorgue(MorgueExportData morgueExportData)
    {
        var morgueFilePath = GetMorgueFilePath();

        var file = new FileInfo(Path.Combine(morgueFilePath, $"Pending{morgueExportData.Id}.txt"));

        if (file.Exists)
            file.Delete();

        var morgueJson = JsonConvert.SerializeObject(morgueExportData);

        using var writer = file.CreateText();

        writer.Write(morgueJson);

        writer.Close();
    }

    public List<MorgueExportData> ReadPendingMorgues()
    {
        var morgueFilePath = GetMorgueFilePath();

        var directory = new DirectoryInfo(morgueFilePath);

        var files = directory.GetFiles("Pending*.txt");

        var morgueExportDataList = new List<MorgueExportData>();

        foreach (var file in files)
        {
            using var reader = file.OpenText();
            var json = reader.ReadToEnd();
            reader.Close();

            try
            {
                morgueExportDataList.Add(JsonConvert.DeserializeObject<MorgueExportData>(json));
            }
            catch (Exception ex)
            {
                var newFilename = file.Name.Replace("Pending", "Error");
                Logger.Warning(ex, $"Could not read pending morgue file {file.Name}. Renaming to {newFilename}.");
                file.CopyTo(Path.Combine(directory.FullName, newFilename), true);
                file.Delete();
            }
        }

        return morgueExportDataList;
    }

    public void DeletePendingMorgue(MorgueExportData morgueExportData)
    {
        var morgueFilePath = GetMorgueFilePath();

        var file = new FileInfo(Path.Combine(morgueFilePath, $"Pending{morgueExportData.Id}.txt"));

        if (file.Exists)
            file.Delete();
    }
}