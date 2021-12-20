using FrigidRogue.MonoGame.Core.UserInterface;

using Microsoft.Xna.Framework.Input;

using SadRogue.Primitives;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Move Right", DefaultKey = Keys.NumPad6)]
    public class MoveRightRequest : MoveRequest
    {
        public MoveRightRequest() : base(Direction.Right)
        {
        }
    }
}