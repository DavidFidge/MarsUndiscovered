using FrigidRogue.MonoGame.Core.UserInterface;

using Microsoft.Xna.Framework.Input;

using SadRogue.Primitives;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Move UpLeft", DefaultKey = Keys.NumPad7)]
    public class MoveUpLeftRequest : MoveRequest
    {
        public MoveUpLeftRequest() : base(Direction.UpLeft)
        {
        }
    }
}