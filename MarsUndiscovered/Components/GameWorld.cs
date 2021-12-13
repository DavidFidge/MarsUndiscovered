using System.Collections.Generic;
using System.Linq;
using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Interfaces;

using GoRogue.Components;
using GoRogue.MapGeneration;

using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Components
{
    public class GameWorld : BaseComponent, IGameWorld
    {
        public GameWorld()
        {
        }

        public List<ComponentTagPair> AllComponents { get; set; }
        public Generator Generator { get; set; }

        public ArrayView<bool> WallsFloors { get; set; }

        public void Generate()
        {
            Logger.Debug("Generating game world");

            var generator = new Generator(50, 50);

            Generator = generator.ConfigAndGenerateSafe(g => g.AddSteps(DefaultAlgorithms.DungeonMazeMapSteps()));

            AllComponents = Generator.Context.ToList();

            WallsFloors = Generator.Context.GetFirst<ArrayView<bool>>();
        }
    }
}