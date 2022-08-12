using FrigidRogue.MonoGame.Core.UserInterface;

using MediatR;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Open World Builder Options", DefaultKey = Keys.Escape)]
    public class OpenWorldBuilderOptionsRequest : IRequest
    {
    }
}