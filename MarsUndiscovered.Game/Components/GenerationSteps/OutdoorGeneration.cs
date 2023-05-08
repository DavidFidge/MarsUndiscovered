using FrigidRogue.MonoGame.Core.Extensions;
using GoRogue.MapGeneration;
using GoRogue.MapGeneration.ConnectionPointSelectors;
using GoRogue.MapGeneration.ContextComponents;
using GoRogue.MapGeneration.Steps;
using GoRogue.MapGeneration.TunnelCreators;
using GoRogue.Random;
using MarsUndiscovered.Game.Components.Maps;
using NGenerics.DataStructures.General;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using ShaiRandom.Generators;
using Point = SadRogue.Primitives.Point;
using Rectangle = SadRogue.Primitives.Rectangle;

namespace MarsUndiscovered.Game.Components.GenerationSteps
{
    public class OutdoorGeneration
    {
        private ushort _fillProbability;
        private int _totalIterations;
        private int _cutoffBigAreaFill;
        private Distance _distanceCalculation;
        private IConnectionPointSelector _connectionPointSelector;
        private ITunnelCreator _tunnelCreationMethod;

        public IEnhancedRandom RNG { get; set; } = GlobalRandom.DefaultRNG;

        public OutdoorGeneration( 
            ushort fillProbability = 60,
            int totalIterations = 7,
            int cutoffBigAreaFill = 2,
            Distance distanceCalculation = null,
            IConnectionPointSelector connectionPointSelector = null,
            ITunnelCreator tunnelCreationMethod = null)
        {
            _fillProbability = fillProbability;
            _totalIterations = totalIterations;
            _cutoffBigAreaFill = cutoffBigAreaFill;
            _distanceCalculation = distanceCalculation;
            _connectionPointSelector = connectionPointSelector;
            _tunnelCreationMethod = tunnelCreationMethod;
        }

        public IEnumerable<GenerationStep> GetSteps()
        {
            Distance dist = _distanceCalculation ?? Distance.Manhattan;
            _connectionPointSelector ??= new RandomConnectionPointSelector(RNG);
            _tunnelCreationMethod ??= new DirectLineTunnelCreator(dist);

            // 1. Randomly fill the map with walls/floors
            yield return new RandomViewFill
            {
                FillProbability = _fillProbability,
                RNG = RNG,
                ExcludePerimeterPoints = false
            };

            // 2. Smooth the map into areas with the cellular automata algorithm
            yield return new CellularAutomataOutdoorGenerator
            {
                AreaAdjacencyRule = dist,
                TotalIterations = _totalIterations,
                CutoffBigAreaFill = _cutoffBigAreaFill,
            };

            // 3. Find all unique areas
            yield return new AreaFinder
            {
                AdjacencyMethod = dist
            };

            // 4. Connect areas by connecting each area to its closest neighbor
            yield return new ClosestMapAreaConnection
            {
                ConnectionPointSelector = _connectionPointSelector,
                DistanceCalc = dist,
                TunnelCreator = _tunnelCreationMethod
            };
        }
    }
}
