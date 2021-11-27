using DavidFidge.MonoGame.Core.UserInterface;

using MediatR;

using Microsoft.Xna.Framework.Input;

namespace Augmented.Messages
{
    [ActionMap(Name = "Increase Game Speed", DefaultKey = Keys.OemPlus)]
    [ActionMap(Name = "Decrease Game Speed", DefaultKey = Keys.OemMinus)]
    [ActionMap(Name = "Pause Game", DefaultKey = Keys.Space)]
    public class ChangeGameSpeedRequest : IRequest
    {
        public int Increment { get; private set; }
        public bool TogglePauseGame { get; private set; }
        public bool Reset { get; private set; }

        public ChangeGameSpeedRequest ResetRequest()
        {
            Reset = true;
            return this;
        }

        public ChangeGameSpeedRequest TogglePauseGameRequest()
        {
            TogglePauseGame = true;
            return this;
        }

        public ChangeGameSpeedRequest IncreaseSpeedRequest()
        {
            Increment = 1;
            return this;
        }

        public ChangeGameSpeedRequest DecreaseSpeedRequest()
        {
            Increment = -1;
            return this;
        }
    }
}