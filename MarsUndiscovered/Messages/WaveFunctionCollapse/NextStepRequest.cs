using FrigidRogue.MonoGame.Core.UserInterface;
using MediatR;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Next Step", DefaultKey = Keys.Space)]
    public class NextStepRequest : IRequest
    {
    }
}