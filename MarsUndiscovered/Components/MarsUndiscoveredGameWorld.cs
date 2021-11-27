using System.Collections.Generic;
using System.Linq;

using Augmented.Graphics.Models;
using Augmented.Graphics.TerrainSpace;
using Augmented.Interfaces;

using DavidFidge.MonoGame.Core.Graphics;
using DavidFidge.MonoGame.Core.Interfaces.Graphics;

using Microsoft.Xna.Framework;

namespace Augmented.Components
{
    public class AugmentedGameWorld : IAugmentedGameWorld
    {
        private readonly IAugmentedEntityFactory _augmentedEntityFactory;
        public ISceneGraph SceneGraph { get; }
        private readonly Terrain _terrain;
        private readonly List<AugmentedEntity> _augmentedEntities = new List<AugmentedEntity>();

        public AugmentedGameWorld(
            IAugmentedEntityFactory augmentedEntityFactory,
            ISceneGraph sceneGraph,
            Terrain terrain
            )
        {
            _augmentedEntityFactory = augmentedEntityFactory;
            SceneGraph = sceneGraph;
            _terrain = terrain;
        }

        public void StartNewGame()
        {
            _terrain.CreateHeightMap(new TerrainParameters(WorldSize.Medium, HillHeight.Medium));

            _augmentedEntities.Add(_augmentedEntityFactory.Create());
            _augmentedEntities.Add(_augmentedEntityFactory.Create());

            _augmentedEntities[0].WorldTransform.ChangeTranslation(new Vector3(50, 0, 0));

            SceneGraph.Initialise(_terrain);

            foreach (var entity in _augmentedEntities)
                SceneGraph.Add(entity, _terrain);

            SceneGraph.LoadContent();
        }

        public void RecreateHeightMap()
        {
            _terrain.CreateHeightMap(new TerrainParameters(WorldSize.Medium, HillHeight.Medium));
            _terrain.LoadContent();
        }

        public void Update()
        {
        }

        public void Select(Ray ray)
        {
            SceneGraph.DeselectAll();

            var selectedEntity = SceneGraph.Select(ray);

            if (selectedEntity != null && selectedEntity is ISelectable selectable)
            {
                selectable.IsSelected = true;
            }
        }

        public void Action(Ray ray)
        {
            var terrainPoint = _terrain.RayToTerrainPoint(ray, SceneGraph);

            if (terrainPoint == null)
                return;

            foreach (var augmentedEntity in _augmentedEntities.Where(e => e.IsSelected))
            {
                augmentedEntity.WorldTransform.ChangeTranslation(terrainPoint.Value);
            }
        }
    }
}