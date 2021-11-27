using DavidFidge.MonoGame.Core.Interfaces.Graphics;

using Microsoft.Xna.Framework;

namespace MarsUndiscovered.Interfaces
{
    public interface IMarsUndiscoveredGameWorld
    {
        ISceneGraph SceneGraph { get; }
        void RecreateHeightMap();
        void Update();
        void Select(Ray ray);
        void Action(Ray ray);
        void StartNewGame();
    }
}