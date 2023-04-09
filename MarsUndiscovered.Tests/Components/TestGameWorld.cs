using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Components;

namespace MarsUndiscovered.Tests.Components
{
    public class TestGameWorld : GameWorld
    {
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
