using FrigidRogue.MonoGame.Core.UserInterface;

using MediatR;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Close Save Game View", DefaultKey = Keys.Escape)]
    public class CloseSaveGameViewRequest : IRequest
    {
    }
}