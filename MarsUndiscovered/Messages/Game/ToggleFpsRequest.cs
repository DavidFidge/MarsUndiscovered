using FrigidRogue.MonoGame.Core.UserInterface;

using MediatR;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Toggle Fps", DefaultKey = Keys.F11)]
    public class ToggleFpsRequest : IRequest
    {
        public ToggleFpsRequest()
        {
        }
    }
}