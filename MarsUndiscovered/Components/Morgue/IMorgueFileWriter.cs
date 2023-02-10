using System.Threading.Tasks;

namespace MarsUndiscovered.Components;

public interface IMorgueFileWriter
{
    Task WriteMorgueTextReportToFile(string morgueTextReport, string username, Guid gameId);
}