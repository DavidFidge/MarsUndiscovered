using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace MarsUndiscovered.Components;

public class MorgueFileWriter : IMorgueFileWriter
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

    public async Task WriteMorgueTextReportToFile(string morgueTextReport, string username, Guid gameId)
    {
        var morgueFilePath = GetMorgueFilePath();
        
        var file = new FileInfo(Path.Combine(morgueFilePath, $"{username} {gameId}.txt"));

        using var writer = file.CreateText();
        
        await writer.WriteAsync(morgueTextReport);
        
        writer.Close();
    }
}