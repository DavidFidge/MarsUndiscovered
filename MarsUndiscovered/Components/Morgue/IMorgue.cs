using System.Threading.Tasks;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Components;

public interface IMorgue 
{
    Task WriteMorgueToFile();
    Task SendMorgueToWeb();
    void Reset();
    void GameStarted();
    void GameEnded();
    void LogActorDeath(Actor actor);
    void SnapshotMorgueExportData(IGameWorld gameWorld, string username);
}