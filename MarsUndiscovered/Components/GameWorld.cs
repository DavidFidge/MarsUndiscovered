using System.Collections.Generic;
using System.Linq;

using MarsUndiscovered.Interfaces;

using GoRogue.Components;
using GoRogue.MapGeneration;

using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Components
{
    public class GameWorld : IGameWorld
    {
        public GameWorld()
        {
        }

        public List<ComponentTagPair> AllComponents { get; set; }
        public Generator Generator { get; set; }

        public ArrayView<bool> WallsFloors { get; set; }

        public void Generate()
        {
            var generator = new Generator(40, 50);

            generator.AddSteps(DefaultAlgorithms.DungeonMazeMapSteps());

            Generator = generator.Generate();

            AllComponents = Generator.Context.ToList();

            WallsFloors = Generator.Context.GetFirst<ArrayView<bool>>();
        }
    }
}