using FrigidRogue.MonoGame.Core.UserInterface;
using Microsoft.Xna.Framework.Input;
using SadRogue.Primitives;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Wait a turn", DefaultKey = Keys.NumPad5)]
    [ActionMap(Name = "Wait a turn Vi", DefaultKey = Keys.OemPeriod)]
    public class MoveWaitRequest : MoveRequest
    {
        public MoveWaitRequest() : base(Direction.None)
        {
        }
    }
}