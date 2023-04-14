using System.Threading.Tasks;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components;

public interface IMorgue 
{
    void Reset();
    void GameStarted();
    void GameEnded();
    void LogActorDeath(Actor actor);
    void SnapshotMorgueExportData(IGameWorld gameWorld, string username, bool uploadMorgueFiles);
    Task SendPendingMorgues();
}