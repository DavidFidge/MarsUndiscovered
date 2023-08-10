using FrigidRogue.MonoGame.Core.UserInterface;
using InputHandlers.Keyboard;
using MediatR;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Wizard Mode Next Level", DefaultKey = Keys.OemPeriod, DefaultKeyboardModifier = KeyboardModifier.Shift)]
    public class WizardModeNextLevelRequest : IRequest
    {
        public WizardModeNextLevelRequest()
        {
        }
    }
}