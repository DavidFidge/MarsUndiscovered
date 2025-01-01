using FrigidRogue.MonoGame.Core.Components.Mediator;
using FrigidRogue.MonoGame.Core.UserInterface;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Close World Builder Options", DefaultKey = Keys.Escape)]
    public class CloseWorldBuilderOptionsRequest : IRequest
    {
    }
}