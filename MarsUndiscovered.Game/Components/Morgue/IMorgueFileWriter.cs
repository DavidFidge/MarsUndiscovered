namespace MarsUndiscovered.Game.Components;

public interface IMorgueFileWriter
{
    void WriteMorgueTextReportToFile(string morgueTextReport, string username, Guid gameId);
    void WritePendingMorgue(MorgueExportData morgueExportData);
    List<MorgueExportData> ReadPendingMorgues();
    void DeletePendingMorgue(MorgueExportData morgueExportData);
}