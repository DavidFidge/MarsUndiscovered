using FrigidRogue.MonoGame.Core.UserInterface;
using MediatR;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Close Square Choice Request", DefaultKey = Keys.Escape)]
    public class CloseSquareChoiceRequest : IRequest
    {
    }
}