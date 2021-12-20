using FrigidRogue.MonoGame.Core.UserInterface;

using Microsoft.Xna.Framework.Input;

using SadRogue.Primitives;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Move Down", DefaultKey = Keys.NumPad2)]
    public class MoveDownRequest : MoveRequest
    {
        public MoveDownRequest() : base(Direction.Down)
        {
        }
    }
}