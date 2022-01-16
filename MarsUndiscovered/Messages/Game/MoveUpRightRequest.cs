using FrigidRogue.MonoGame.Core.UserInterface;

using Microsoft.Xna.Framework.Input;

using SadRogue.Primitives;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Move UpRight", DefaultKey = Keys.NumPad9)]
    [ActionMap(Name = "Move UpRight Vi", DefaultKey = Keys.U)]
    public class MoveUpRightRequest : MoveRequest
    {
        public MoveUpRightRequest() : base(Direction.UpRight)
        {
        }
    }
}