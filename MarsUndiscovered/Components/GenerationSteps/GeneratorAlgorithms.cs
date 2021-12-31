using System.Collections.Generic;
using GoRogue.MapGeneration;
using GoRogue.MapGeneration.ConnectionPointSelectors;
using GoRogue.MapGeneration.Steps;
using GoRogue.MapGeneration.TunnelCreators;
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
            int cutoffBigAreaFill = 2,
            int border = 2,
            Distance? distanceCalculation = null,
            IConnectionPointSelector? connectionPointSelector = null,
            ITunnelCreator? tunnelCreationMethod = null)
        {
            rng ??= GlobalRandom.DefaultRNG;
            Distance dist = distanceCalculation ?? Distance.Manhattan;
            connectionPointSelector ??= new RandomConnectionPointSelector(rng);
            tunnelCreationMethod ??= new DirectLineTunnelCreator(dist);

            // 1. Randomly fill the map with walls/floors
            yield return new RandomViewFill
            {
                FillProbability = fillProbability,
                RNG = rng,
                ExcludePerimeterPoints = false
            };

            // 2. Smooth the map into areas with the cellular automata algorithm
            yield return new CellularAutomataAreaGeneration
            {
                AreaAdjacencyRule = dist,
                TotalIterations = totalIterations,
                CutoffBigAreaFill = cutoffBigAreaFill,
            };

            // 3. Set borders to floors
            yield return new BorderGenerationStep
            {
                Border = border
            };

            // 4. Find all unique areas
            yield return new AreaFinder
            {
                AdjacencyMethod = dist
            };

            // 5. Connect areas by connecting each area to its closest neighbor
            yield return new ClosestMapAreaConnection
            {
                ConnectionPointSelector = connectionPointSelector,
                DistanceCalc = dist,
                TunnelCreator = tunnelCreationMethod
            };
        }
    }
}