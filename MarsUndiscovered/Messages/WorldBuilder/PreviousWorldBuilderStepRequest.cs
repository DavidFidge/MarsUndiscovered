using FrigidRogue.MonoGame.Core.UserInterface;
using MediatR;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Views
{
    [ActionMap(Name = "Previous World Builder Step", DefaultKey = Keys.Left)]
    public class PreviousWorldBuilderStepRequest : IRequest
    {
    }
}