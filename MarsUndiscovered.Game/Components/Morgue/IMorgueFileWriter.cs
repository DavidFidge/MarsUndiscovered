using System.Threading.Tasks;

namespace MarsUndiscovered.Game.Components;

public interface IMorgueFileWriter
{
    Task WriteMorgueTextReportToFile(string morgueTextReport, string username, Guid gameId);
}