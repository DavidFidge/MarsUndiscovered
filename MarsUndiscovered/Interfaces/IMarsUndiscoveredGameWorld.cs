using FrigidRogue.MonoGame.Core.Interfaces.Graphics;

namespace MarsUndiscovered.Interfaces
{
    public interface IMarsUndiscoveredGameWorld
    {
        ISceneGraph SceneGraph { get; }
        void Update();
        void StartNewGame();
    }
}