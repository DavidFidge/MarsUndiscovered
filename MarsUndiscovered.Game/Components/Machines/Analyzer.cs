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
    }
}