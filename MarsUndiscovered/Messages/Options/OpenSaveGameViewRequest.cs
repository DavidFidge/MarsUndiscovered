using FrigidRogue.MonoGame.Core.Components.Mediator;
using FrigidRogue.MonoGame.Core.UserInterface;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Open Save Game View", DefaultKey = Keys.S)]

    public class OpenSaveGameViewRequest : IRequest
    {
    }
}