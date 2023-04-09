using System.Threading.Tasks;

namespace MarsUndiscovered.Game.Components;

public interface IMorgueWebService
{
    Task SendMorgue(MorgueExportData morgueExportData);
}