using FrigidRogue.MonoGame.Core.UserInterface;
using Microsoft.Xna.Framework.Input;
using SadRogue.Primitives;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Move Right", DefaultKey = Keys.NumPad6)]
    [ActionMap(Name = "Move Right Arrow", DefaultKey = Keys.Right)]
    [ActionMap(Name = "Move Right Vi", DefaultKey = Keys.L)]
    public class MoveRightRequest : MoveRequest
    {
        public MoveRightRequest() : base(Direction.Right)
        {
        }
    }
}