using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FrigidRogue.MonoGame.Core.Components;

using ShaiRandom.Generators;

namespace MarsUndiscovered.Tests.Components
{
    public class TestContextualEnhancedRandom : ContextualEnhancedRandom
    {
        public Dictionary<string, KnownSeriesRandom> KnownSeries { get; } = new Dictionary<string, KnownSeriesRandom>();

        public override bool NextBool(string context)
        {
            if (KnownSeries.TryGetValue(context, out var knownSeriesRandom))
                return knownSeriesRandom.NextBool();

            return base.NextBool(context);
        }

        public override int NextInt(int upper, string context)
        {
            if (KnownSeries.TryGetValue(context, out var knownSeriesRandom))
                return knownSeriesRandom.NextInt(upper);

            return base.NextInt(upper);
        }

        public override int NextInt(int lower, int upper, string context)
        {
            if (KnownSeries.TryGetValue(context, out var knownSeriesRandom))
                return knownSeriesRandom.NextInt(lower, upper);

            return base.NextInt(lower, upper);
        }
    }
}
