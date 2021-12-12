using FrigidRogue.MonoGame.Core.UserInterface;

using MediatR;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "New Game", DefaultKey = Keys.N)]
    public class NewGameRequest : IRequest
    {
    }
}