using FrigidRogue.MonoGame.Core.UserInterface;
using MediatR;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Open Save Game View", DefaultKey = Keys.S)]

    public class OpenSaveGameViewRequest : IRequest
    {
    }
}