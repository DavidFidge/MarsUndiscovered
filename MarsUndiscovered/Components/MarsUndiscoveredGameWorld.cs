using System.Collections.Generic;
using System.Linq;

using MarsUndiscovered.Interfaces;

using FrigidRogue.MonoGame.Core.Interfaces.Graphics;

namespace MarsUndiscovered.Components
{
    public class MarsUndiscoveredGameWorld : IMarsUndiscoveredGameWorld
    {
        public ISceneGraph SceneGraph { get; }

        public MarsUndiscoveredGameWorld(ISceneGraph sceneGraph)
        {
            SceneGraph = sceneGraph;
        }

        public void StartNewGame()
        {
            // SceneGraph.Initialise(_terrain);
            // SceneGraph.LoadContent();
        }

        public void Update()
        {
        }
    }
}