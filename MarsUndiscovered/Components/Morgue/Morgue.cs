using System.Text;
using System.Threading.Tasks;
using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Extensions;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Services;
using MarsUndiscovered.Components.SaveData;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Components;

public class Morgue : BaseComponent, IMorgue, ISaveable
{
    private readonly IMorgueFileWriter _morgueFileWriter;
    private readonly IMorgueWebService _morgueWebService;
    private MorgueSaveData _morgueSaveData;
    private MorgueExportData _morgueExportData;

    public Morgue(
        IMorgueFileWriter morgueFileWriter,
        IMorgueWebService morgueWebService)
    {
        _morgueFileWriter = morgueFileWriter;
        _morgueWebService = morgueWebService;
        _morgueSaveData = new MorgueSaveData();
    }

    public async Task WriteMorgueToFile()
    {
        if (_morgueExportData != null)
        {
            var morgueExportDataClone = (MorgueExportData)_morgueExportData.Clone();

            try
            {
                await _morgueFileWriter.WriteMorgueTextReportToFile(morgueExportDataClone.TextReport, morgueExportDataClone.Username,
                    morgueExportDataClone.Id);
            }
            catch (Exception ex)
            {
                Logger.Warning(ex, "Error while writing morgue to file");
            }
        }
        else
        {
            Logger.Warning("Morgue snapshot is null. Call SnapshotMorgueExportData first.");
        }
    }

    public async Task SendMorgueToWeb()
    {
        if (_morgueExportData != null)
        {
            var morgueExportDataClone = (MorgueExportData)_morgueExportData.Clone();
            
            try
            {
                await _morgueWebService.SendMorgue(morgueExportDataClone);
            }
            catch (Exception ex)
            {
                Logger.Warning(ex, "Error while sending morgue file to web site");
            }
        }
        else
        {
            Logger.Warning("Morgue snapshot is null. Call SnapshotMorgueExportData first.");
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
            GameEndStatus = gameWorld.Player.IsDead ? $"You were {gameWorld.Player.IsDeadMessage}" : "Retrieved ship parts",
            IsVictorious = gameWorld.Player.IsVictorious,
            Version = 1
        };
        
        return morgueExportData;
    }

    public void Reset()
    {
        _morgueSaveData = new MorgueSaveData();
        _morgueExportData = null;
    }

    public void GameStarted()
    {
        _morgueSaveData.StartDate = DateTimeProvider.UtcNow;
    }
    
    public void GameEnded()
    {
        _morgueSaveData.EndDate = DateTimeProvider.UtcNow;
    }

    public void SnapshotMorgueExportData(IGameWorld gameWorld, string username)
    {
        _morgueExportData = null;

        if (!gameWorld.Player.IsGameEndState)
        {
            Logger.Warning($"Cannot snapshot morgue data - player is not dead and victory has not been achieved.");
            return;
        }

        _morgueExportData = CreateMorgueExportData(gameWorld, username);
        var morgueTextReport = BuildTextReport(_morgueExportData);
        _morgueExportData.TextReport = morgueTextReport.ToString();
    }
    
    public void SaveState(ISaveGameService saveGameService)
    {
        var memento = new Memento<MorgueSaveData>();
        
        saveGameService.SaveToStore(memento);
    }

    public void LoadState(ISaveGameService saveGameService)
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

        morgueText.AppendLine($"Mars Undiscovered");
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