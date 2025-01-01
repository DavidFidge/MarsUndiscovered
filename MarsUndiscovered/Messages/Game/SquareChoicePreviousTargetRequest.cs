using FrigidRogue.MonoGame.Core.Components.Mediator;
using FrigidRogue.MonoGame.Core.UserInterface;
using InputHandlers.Keyboard;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Square Choice Previous Target Request", DefaultKey = Keys.Tab, DefaultKeyboardModifier = KeyboardModifier.Shift)]
    public class SquareChoicePreviousTargetRequest : IRequest
    {
    }
}