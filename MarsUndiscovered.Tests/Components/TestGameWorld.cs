using FrigidRogue.MonoGame.Core.Components;

using GoRogue.Random;

using MarsUndiscovered.Game.Components;

using ShaiRandom.Generators;

namespace MarsUndiscovered.Tests.Components
{
    public class TestGameWorld : GameWorld
    {
        public TestContextualEnhancedRandom TestContextualEnhancedRandom
        {
            get { return _contextualEnhancedRandom as TestContextualEnhancedRandom; }
            set { _contextualEnhancedRandom = value; }
        }

        public TestGameWorld() : base()
        {
            _contextualEnhancedRandom = new TestContextualEnhancedRandom();

            GlobalRandom.DefaultRNG = _contextualEnhancedRandom;
        }

        public IEnumerable<CommandResult> TestNextTurn()
        {
            return NextTurn();
        }

        public void TestResetFieldOfView()
        {
            ResetFieldOfView();
        }
    }
}
