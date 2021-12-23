using FrigidRogue.MonoGame.Core.UserInterface;

using Microsoft.Xna.Framework.Input;

using SadRogue.Primitives;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Move DownLeft", DefaultKey = Keys.NumPad1)]
    public class MoveDownLeftRequest : MoveRequest
    {
        public MoveDownLeftRequest() : base(Direction.DownLeft)
        {
        }
    }
}