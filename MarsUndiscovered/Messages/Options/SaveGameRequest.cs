using FrigidRogue.MonoGame.Core.UserInterface;
using MediatR;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Save Game", DefaultKey = Keys.S)]
    public class SaveGameRequest : IRequest
    {
    }
}