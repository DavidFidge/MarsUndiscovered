using System.Collections.Generic;
using System.Linq;

using MarsUndiscovered.Graphics.Models;
using MarsUndiscovered.Graphics.TerrainSpace;
using MarsUndiscovered.Interfaces;

using DavidFidge.MonoGame.Core.Graphics;
using DavidFidge.MonoGame.Core.Interfaces.Graphics;

using Microsoft.Xna.Framework;

namespace MarsUndiscovered.Components
{
    public class MarsUndiscoveredGameWorld : IMarsUndiscoveredGameWorld
    {
        private readonly IMarsUndiscoveredEntityFactory _marsUndiscoveredEntityFactory;
        public ISceneGraph SceneGraph { get; }
        private readonly Terrain _terrain;
        private readonly List<MarsUndiscoveredEntity> _marsUndiscoveredEntities = new List<MarsUndiscoveredEntity>();

        public MarsUndiscoveredGameWorld(
            IMarsUndiscoveredEntityFactory marsUndiscoveredEntityFactory,
            ISceneGraph sceneGraph,
            Terrain terrain
            )
        {
            _marsUndiscoveredEntityFactory = marsUndiscoveredEntityFactory;
            SceneGraph = sceneGraph;
            _terrain = terrain;
        }

        public void StartNewGame()
        {
            _terrain.CreateHeightMap(new TerrainParameters(WorldSize.Medium, HillHeight.Medium));

            _marsUndiscoveredEntities.Add(_marsUndiscoveredEntityFactory.Create());
            _marsUndiscoveredEntities.Add(_marsUndiscoveredEntityFactory.Create());

            _marsUndiscoveredEntities[0].WorldTransform.ChangeTranslation(new Vector3(50, 0, 0));

            SceneGraph.Initialise(_terrain);

            foreach (var entity in _marsUndiscoveredEntities)
                SceneGraph.Add(entity, _terrain);

            SceneGraph.LoadContent();
        }

        public void Update()
        {
        }
    }
}