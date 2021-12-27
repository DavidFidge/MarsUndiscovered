using FrigidRogue.MonoGame.Core.UserInterface;
using MediatR;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Overwrite Save Game", DefaultKey = Keys.O)]

    public class OverwriteSaveGameRequest : IRequest
    {
    }
}