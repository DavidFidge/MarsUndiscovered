using FrigidRogue.MonoGame.Core.Interfaces.Graphics;

using Microsoft.Xna.Framework;

namespace MarsUndiscovered.Interfaces
{
    public interface IMarsUndiscoveredGameWorld
    {
        ISceneGraph SceneGraph { get; }
        void Update();
        void StartNewGame();
    }
}