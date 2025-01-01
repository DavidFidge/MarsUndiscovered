using FrigidRogue.MonoGame.Core.Components.Mediator;
using FrigidRogue.MonoGame.Core.UserInterface;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Open Load Game View", DefaultKey = Keys.L)]

    public class OpenLoadGameViewRequest : IRequest
    {
    }
}