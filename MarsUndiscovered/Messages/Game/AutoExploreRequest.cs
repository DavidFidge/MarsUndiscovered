using FrigidRogue.MonoGame.Core.UserInterface;

using MediatR;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Auto Explore", DefaultKey = Keys.X)]
    public class AutoExploreRequest : IRequest
    {
        public AutoExploreRequest()
        {
        }
    }
}