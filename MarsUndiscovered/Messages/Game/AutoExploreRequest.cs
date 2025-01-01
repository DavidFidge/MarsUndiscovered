using FrigidRogue.MonoGame.Core.Components.Mediator;
using FrigidRogue.MonoGame.Core.UserInterface;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Auto Explore", DefaultKey = Keys.X)]
    public class AutoExploreRequest : IRequest
    {
    }
}