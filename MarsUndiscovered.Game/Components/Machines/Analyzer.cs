using Microsoft.Xna.Framework;

namespace MarsUndiscovered.Game.Components
{
    public class Analyzer : MachineType
    {
        public Analyzer()
        {
            ForegroundColour = Color.Yellow;
            BackgroundColour = Color.Cyan;
            AsciiCharacter = '?';
        }

        public override string GetDescription()
        {
            return "An analyzer machine";
        }

        public override string GetLongDescription()
        {
            return "An analyzer machine that can be used to identify unknown items";
        }
    }
}