using FrigidRogue.MonoGame.Core.UserInterface;

using Microsoft.Xna.Framework.Input;

using SadRogue.Primitives;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Move Left", DefaultKey = Keys.NumPad4)]
    public class MoveLeftRequest : MoveRequest
    {
        public MoveLeftRequest() : base(Direction.Left)
        {
        }
    }
}