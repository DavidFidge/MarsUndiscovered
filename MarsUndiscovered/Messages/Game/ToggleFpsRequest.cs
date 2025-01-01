using FrigidRogue.MonoGame.Core.Components.Mediator;
using FrigidRogue.MonoGame.Core.UserInterface;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Toggle Fps", DefaultKey = Keys.F11)]
    public class ToggleFpsRequest : IRequest
    {
    }
}