using System.Threading.Tasks;

namespace MarsUndiscovered.Components;

public interface IMorgueWebService
{
    Task SendMorgue(MorgueExportData morgueExportData);
}