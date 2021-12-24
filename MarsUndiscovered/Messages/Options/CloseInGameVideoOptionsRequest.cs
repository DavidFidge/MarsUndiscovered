using FrigidRogue.MonoGame.Core.UserInterface;

using MediatR;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Close In Game Video Options", DefaultKey = Keys.Escape)]
    public class CloseInGameVideoOptionsRequest : IRequest
    {
    }
}