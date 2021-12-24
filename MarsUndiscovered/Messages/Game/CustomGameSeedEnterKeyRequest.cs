using FrigidRogue.MonoGame.Core.UserInterface;

using MediatR;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "CustomGameSeedEnterKeyRequest", DefaultKey = Keys.Enter)]
    public class CustomGameSeedEnterKeyRequest : IRequest
    {
    }
}