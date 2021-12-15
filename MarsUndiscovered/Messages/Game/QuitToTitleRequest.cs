using FrigidRogue.MonoGame.Core.UserInterface;
using MediatR;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Exit Game", DefaultKey = Keys.X)]
    public class QuitToTitleRequest : IRequest
    {
    }
}