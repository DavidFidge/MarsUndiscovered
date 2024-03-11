using FrigidRogue.MonoGame.Core.UserInterface;

using MediatR;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "HotBar Slot 0", DefaultKey = Keys.D0)]
    [ActionMap(Name = "HotBar Slot 1", DefaultKey = Keys.D1)]
    [ActionMap(Name = "HotBar Slot 2", DefaultKey = Keys.D2)]
    [ActionMap(Name = "HotBar Slot 3", DefaultKey = Keys.D3)]
    [ActionMap(Name = "HotBar Slot 4", DefaultKey = Keys.D4)]
    [ActionMap(Name = "HotBar Slot 5", DefaultKey = Keys.D5)]
    [ActionMap(Name = "HotBar Slot 6", DefaultKey = Keys.D6)]
    [ActionMap(Name = "HotBar Slot 7", DefaultKey = Keys.D7)]
    [ActionMap(Name = "HotBar Slot 8", DefaultKey = Keys.D8)]
    [ActionMap(Name = "HotBar Slot 9", DefaultKey = Keys.D9)]
    public class HotBarItemRequest : IRequest
    {
        public Keys Key { get; set; }
        public HotBarItemRequest(Keys key)
        {
            Key = key;
        }
    }
}