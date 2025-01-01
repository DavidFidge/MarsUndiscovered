using FrigidRogue.MonoGame.Core.Components.Mediator;
using FrigidRogue.MonoGame.Core.UserInterface;
using InputHandlers.Keyboard;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Wizard Mode Previous Level", DefaultKey = Keys.OemComma, DefaultKeyboardModifier = KeyboardModifier.Shift)]
    public class WizardModePreviousLevelRequest : IRequest
    {
    }
}