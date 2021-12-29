using System.Collections.Generic;
using GoRogue.MapGeneration;
using GoRogue.MapGeneration.Steps;
using GoRogue.Random;
using SadRogue.Primitives;
using Troschuetz.Random;

namespace MarsUndiscovered.Components.GenerationSteps
{
    public static class GeneratorAlgorithms
    {
        public static IEnumerable<GenerationStep> OutdoorGeneneration(
            IGenerator rng = null,
            ushort fillProbability = 60,
            int totalIterations = 7,
            int cutoffBigAreaFill = 4,
            int border = 3,
            Distance? distanceCalculation = null)
        {
            rng ??= GlobalRandom.DefaultRNG;
            Distance dist = distanceCalculation ?? Distance.Manhattan;

            //// 1. Randomly fill the map with walls/floors
            yield return new RandomViewFill
            {
                FillProbability = fillProbability,
                RNG = rng,
                ExcludePerimeterPoints = false
            };

            //// 2. Smooth the map into areas with the cellular automata algorithm
            yield return new CellularAutomataAreaGeneration
            {
                AreaAdjacencyRule = dist,
                TotalIterations = totalIterations,
                CutoffBigAreaFill = cutoffBigAreaFill,
            };

            //// 3. Set borders to floors
            yield return new BorderGenerationStep
            {
                Border = border
            };
        }
    }
}