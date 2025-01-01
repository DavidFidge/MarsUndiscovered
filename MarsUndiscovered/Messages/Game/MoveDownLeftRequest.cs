using FrigidRogue.MonoGame.Core.UserInterface;
using Microsoft.Xna.Framework.Input;
using SadRogue.Primitives;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Move DownLeft", DefaultKey = Keys.NumPad1)]
    [ActionMap(Name = "Move DownLeft Vi", DefaultKey = Keys.B)]
    public class MoveDownLeftRequest : MoveRequest
    {
        public MoveDownLeftRequest() : base(Direction.DownLeft)
        {
        }
    }
}