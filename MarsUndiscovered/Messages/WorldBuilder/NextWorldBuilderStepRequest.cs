using FrigidRogue.MonoGame.Core.UserInterface;
using MediatR;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Views
{
    [ActionMap(Name = "Next World Builder Step", DefaultKey = Keys.Right)]
    [ActionMap(Name = "Next World Builder Step Space", DefaultKey = Keys.Space)]
    public class NextWorldBuilderStepRequest : IRequest
    {
    }
}