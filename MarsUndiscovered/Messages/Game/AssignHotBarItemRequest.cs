using FrigidRogue.MonoGame.Core.Components.Mediator;
using FrigidRogue.MonoGame.Core.UserInterface;
using InputHandlers.Keyboard;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Assign HotBar Slot 0", DefaultKey = Keys.D0, DefaultKeyboardModifier = KeyboardModifier.Ctrl)]
    [ActionMap(Name = "Assign HotBar Slot 1", DefaultKey = Keys.D1, DefaultKeyboardModifier = KeyboardModifier.Ctrl)]
    [ActionMap(Name = "Assign HotBar Slot 2", DefaultKey = Keys.D2, DefaultKeyboardModifier = KeyboardModifier.Ctrl)]
    [ActionMap(Name = "Assign HotBar Slot 3", DefaultKey = Keys.D3, DefaultKeyboardModifier = KeyboardModifier.Ctrl)]
    [ActionMap(Name = "Assign HotBar Slot 4", DefaultKey = Keys.D4, DefaultKeyboardModifier = KeyboardModifier.Ctrl)]
    [ActionMap(Name = "Assign HotBar Slot 5", DefaultKey = Keys.D5, DefaultKeyboardModifier = KeyboardModifier.Ctrl)]
    [ActionMap(Name = "Assign HotBar Slot 6", DefaultKey = Keys.D6, DefaultKeyboardModifier = KeyboardModifier.Ctrl)]
    [ActionMap(Name = "Assign HotBar Slot 7", DefaultKey = Keys.D7, DefaultKeyboardModifier = KeyboardModifier.Ctrl)]
    [ActionMap(Name = "Assign HotBar Slot 8", DefaultKey = Keys.D8, DefaultKeyboardModifier = KeyboardModifier.Ctrl)]
    [ActionMap(Name = "Assign HotBar Slot 9", DefaultKey = Keys.D9, DefaultKeyboardModifier = KeyboardModifier.Ctrl)]
    public class AssignHotBarItemRequest : IRequest
    {
        public Keys Key { get; set; }
        public AssignHotBarItemRequest(Keys key)
        {
            Key = key;
        }
    }
}