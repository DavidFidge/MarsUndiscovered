using FrigidRogue.MonoGame.Core.UserInterface;

using MediatR;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Close Video Options", DefaultKey = Keys.Escape)]
    public class CloseVideoOptionsRequest : IRequest
    {
    }
}