using FrigidRogue.MonoGame.Core.UserInterface;
using MediatR;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Views
{
    [ActionMap(Name = "Next World Builder Step", DefaultKey = Keys.Right)]
    public class NextWorldBuilderStepRequest : IRequest
    {
    }
}