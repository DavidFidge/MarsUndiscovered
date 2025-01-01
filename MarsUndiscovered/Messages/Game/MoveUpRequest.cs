using FrigidRogue.MonoGame.Core.UserInterface;
using Microsoft.Xna.Framework.Input;
using SadRogue.Primitives;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Move Up", DefaultKey = Keys.NumPad8)]
    [ActionMap(Name = "Move Up Arrow", DefaultKey = Keys.Up)]
    [ActionMap(Name = "Move Up Vi", DefaultKey = Keys.K)]
    public class MoveUpRequest : MoveRequest
    {
        public MoveUpRequest() : base(Direction.Up)
        {
        }
    }
}