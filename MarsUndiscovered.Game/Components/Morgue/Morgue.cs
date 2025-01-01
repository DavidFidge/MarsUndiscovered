using System.Text;
using System.Threading.Tasks;
using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Extensions;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Services;
using MarsUndiscovered.Game.Components.SaveData;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components;

public class Morgue : BaseComponent, IMorgue, ISaveable
{
    private readonly IMorgueFileWriter _morgueFileWriter;
    private readonly IMorgueWebService _morgueWebService;
    private MorgueSaveData _morgueSaveData;

    public Morgue(
        IMorgueFileWriter morgueFileWriter,
        IMorgueWebService morgueWebService)
    {
        _morgueFileWriter = morgueFileWriter;
        _morgueWebService = morgueWebService;
        _morgueSaveData = new MorgueSaveData();
    }

    private async Task SendMorgueExportDataToWeb(MorgueExportData morgueExportData)
    {
        var success = true;

        try
        {
            await _morgueWebService.SendMorgue(morgueExportData);
        }
        catch (Exception ex)
        {
            Logger.Warning(ex, "Error while sending morgue file to web site.");
            success = false;
        }

        if (success)
        {
            try
            {
                _morgueFileWriter.DeletePendingMorgue(morgueExportData);
            }
            catch (Exception ex)
            {
                Logger.Warning(ex, "Unable to delete pending morgue file.");
            }
        }
    }

    private MorgueExportData CreateMorgueExportData(IGameWorld gameWorld, string username)
    {
        var morgueExportData = new MorgueExportData
        {
            Id = gameWorld.GameId,
            Username = username,
            Seed = gameWorld.Seed.ToString(),
            StartDate = _morgueSaveData.StartDate,
            EndDate = _morgueSaveData.EndDate,
            EnemiesDefeated = _morgueSaveData.EnemiesDefeated.ToDictionary(x => x.Key, x => x.Value),
            FinalInventory = gameWorld.Inventory.GetInventoryItems().Select(i => i.ItemDiscoveredDescription).ToList(),
            Health = gameWorld.Player.Health,
            MaxHealth = gameWorld.Player.MaxHealth,
            GameEndStatus = gameWorld.Player.IsDead ? $"You were {gameWorld.Player.IsDeadMessage}" : "You retrieved ship parts",
            IsVictorious = gameWorld.Player.IsVictorious,
            Version = 1
        };
        
        return morgueExportData;
    }

    public void Reset()
    {
        _morgueSaveData = new MorgueSaveData();
    }

    public void GameStarted()
    {
        _morgueSaveData.StartDate = DateTimeProvider.UtcNow;
    }
    
    public void GameEnded()
    {
        _morgueSaveData.EndDate = DateTimeProvider.UtcNow;
    }

    public void SnapshotMorgueExportData(IGameWorld gameWorld, string username, bool uploadMorgueFiles)
    {
        if (!gameWorld.Player.IsGameEndState)
        {
            Logger.Warning("Cannot snapshot morgue data - player is not dead and victory has not been achieved.");
            return;
        }

        var morgueExportData = CreateMorgueExportData(gameWorld, username);

        var morgueTextReport = BuildTextReport(morgueExportData);
        morgueExportData.TextReport = morgueTextReport.ToString();

        WriteMorgueTextReportToFile(morgueExportData);

        if (uploadMorgueFiles)
            WritePendingMorgue(morgueExportData);
    }

    private void WritePendingMorgue(MorgueExportData morgueExportData)
    {
        try
        {
            _morgueFileWriter.WritePendingMorgue(morgueExportData);
        }
        catch (Exception ex)
        {
            Logger.Warning(ex, "Error while writing pending morgue to file. Morgue will not be sent to web service.");
        }
    }

    private void WriteMorgueTextReportToFile(MorgueExportData morgueExportData)
    {
        try
        {
            _morgueFileWriter.WriteMorgueTextReportToFile(morgueExportData.TextReport, morgueExportData.Username,
                morgueExportData.Id);
        }
        catch (Exception ex)
        {
            Logger.Warning(ex, "Error while writing morgue report to file");
        }
    }

    public async Task SendPendingMorgues()
    {
        try
        {
            var failedMorgues = _morgueFileWriter.ReadPendingMorgues();

            foreach (var morgueExportData in failedMorgues)
            {
                await SendMorgueExportDataToWeb(morgueExportData);
            }
        }
        catch (Exception ex)
        {
            Logger.Warning(ex, "Error while sending pending morgues");
        }
    }

    public void SaveState(ISaveGameService saveGameService, IGameWorld gameWorld)
    {
        var memento = new Memento<MorgueSaveData>();
        
        saveGameService.SaveToStore(memento);
    }

    public void LoadState(ISaveGameService saveGameService, IGameWorld gameWorld)
    {
        var memento = saveGameService.GetFromStore<MorgueSaveData>();
        _morgueSaveData = memento.State;
    }

    public void LogActorDeath(Actor actor)
    {
        if (actor is Player)
            return;
        
        if (!_morgueSaveData.EnemiesDefeated.TryAdd(actor.Name, 1))
            _morgueSaveData.EnemiesDefeated[actor.Name]++;
    }

    private StringBuilder BuildTextReport(MorgueExportData morgueExportData)
    {
        var morgueText = new StringBuilder();

        morgueText.AppendLine("Mars Undiscovered");
        morgueText.AppendLine($"Game ID: {morgueExportData.Id}");
        morgueText.AppendLine($"Game Version: {morgueExportData.GameVersion}");
        morgueText.AppendLine($"Seed: {morgueExportData.Seed}");
        morgueText.AppendLine($"Username: {morgueExportData.Username}");
        morgueText.AppendLine($"Start Date: {morgueExportData.StartDate:yyyy-MM-dd hh:mm:ss} UTC");
        morgueText.AppendLine($"End Date: {morgueExportData.EndDate:yyyy-MM-dd hh:mm:ss} UTC");
        morgueText.AppendLine();
        morgueText.AppendLine($"{(morgueExportData.IsVictorious ? "Won" : "Died")}: {morgueExportData.GameEndStatus}");
        morgueText.AppendLine();
        AppendHeader(morgueText, "STATUS");
        morgueText.AppendLine($"Health {morgueExportData.Health}/{morgueExportData.MaxHealth}");
        morgueText.AppendLine();
        AppendHeader(morgueText, "INVENTORY");
        
        if (morgueExportData.FinalInventory.IsEmpty())
            morgueText.AppendLine("No items in inventory");

        foreach (var item in morgueExportData.FinalInventory)
            morgueText.AppendLine(item);
        
        morgueText.AppendLine();
        AppendHeader(morgueText, "ENEMIES DEFEATED");
        
        if (morgueExportData.EnemiesDefeated.IsEmpty())
            morgueText.AppendLine("No enemies were defeated");

        foreach (var item in morgueExportData.EnemiesDefeated)
            morgueText.AppendLine($"{item.Value} {item.Key}");

        return morgueText;
    }
    
    private void AppendHeader(StringBuilder stringBuilder, string headerText)
    {
        stringBuilder.AppendLine(headerText);
        stringBuilder.AppendLine("--------------------------------------------------------------------------------");
    }
}