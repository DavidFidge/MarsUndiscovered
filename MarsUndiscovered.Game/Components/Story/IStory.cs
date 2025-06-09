using FrigidRogue.MonoGame.Core.Interfaces.Services;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components;

public interface IStory
{
    void Initialize(IGameWorld gameWorld);
    void NextTurn();
    void SaveState(ISaveGameService saveGameService, IGameWorld gameWorld);
    void LoadState(ISaveGameService saveGameService, IGameWorld gameWorld);
}