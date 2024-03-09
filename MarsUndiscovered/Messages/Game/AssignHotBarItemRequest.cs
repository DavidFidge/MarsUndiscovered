using FrigidRogue.MonoGame.Core.UserInterface;
using InputHandlers.Keyboard;
using MediatR;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "0", DefaultKey = Keys.D0, DefaultKeyboardModifier = KeyboardModifier.Ctrl)]
    [ActionMap(Name = "1", DefaultKey = Keys.D1, DefaultKeyboardModifier = KeyboardModifier.Ctrl)]
    [ActionMap(Name = "2", DefaultKey = Keys.D2, DefaultKeyboardModifier = KeyboardModifier.Ctrl)]
    [ActionMap(Name = "3", DefaultKey = Keys.D3, DefaultKeyboardModifier = KeyboardModifier.Ctrl)]
    [ActionMap(Name = "4", DefaultKey = Keys.D4, DefaultKeyboardModifier = KeyboardModifier.Ctrl)]
    [ActionMap(Name = "5", DefaultKey = Keys.D5, DefaultKeyboardModifier = KeyboardModifier.Ctrl)]
    [ActionMap(Name = "6", DefaultKey = Keys.D6, DefaultKeyboardModifier = KeyboardModifier.Ctrl)]
    [ActionMap(Name = "7", DefaultKey = Keys.D7, DefaultKeyboardModifier = KeyboardModifier.Ctrl)]
    [ActionMap(Name = "8", DefaultKey = Keys.D8, DefaultKeyboardModifier = KeyboardModifier.Ctrl)]
    [ActionMap(Name = "9", DefaultKey = Keys.D9, DefaultKeyboardModifier = KeyboardModifier.Ctrl)]
    public class AssignHotBarItemRequest : IRequest
    {
        public Keys Key { get; set; }
        public AssignHotBarItemRequest(Keys key)
        {
            Key = key;
        }
    }
}