using FrigidRogue.MonoGame.Core.Components.Mediator;
using FrigidRogue.MonoGame.Core.UserInterface;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Close Load Game View", DefaultKey = Keys.Escape)]

    public class CloseLoadGameViewRequest : IRequest
    {
    }
}