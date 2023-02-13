using System.Threading.Tasks;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Components;

public interface IMorgue 
{
    Task WriteMorgueToFile(Guid gameId);
    Task SendMorgueToWeb(Guid gameId);
    void Reset();
    void GameStarted();
    void GameEnded();
    void LogActorDeath(Actor actor);
    void SnapshotMorgueExportData(IGameWorld gameWorld, string username);
}