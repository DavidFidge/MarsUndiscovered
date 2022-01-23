using FrigidRogue.MonoGame.Core.UserInterface;

using Microsoft.Xna.Framework.Input;

using SadRogue.Primitives;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Move Left", DefaultKey = Keys.NumPad4)]
    [ActionMap(Name = "Move Left Arrow", DefaultKey = Keys.Left)]
    [ActionMap(Name = "Move Left Vi", DefaultKey = Keys.H)]
    public class MoveLeftRequest : MoveRequest
    {
        public MoveLeftRequest() : base(Direction.Left)
        {
        }
    }
}