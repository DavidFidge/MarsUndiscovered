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
            {
                return knownSeriesRandom.NextBool();
            }

            return base.NextBool(context);
        }
    }
}
