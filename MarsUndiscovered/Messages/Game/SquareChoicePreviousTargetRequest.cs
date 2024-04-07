using FrigidRogue.MonoGame.Core.UserInterface;
using InputHandlers.Keyboard;
using MediatR;
using Microsoft.Xna.Framework.Input;
using SadRogue.Primitives;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Square Choice Previous Target Request", DefaultKey = Keys.Tab, DefaultKeyboardModifier = KeyboardModifier.Shift)]
    public class SquareChoicePreviousTargetRequest : IRequest
    {
    }
}