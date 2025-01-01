using FrigidRogue.MonoGame.Core.Components.Mediator;
using FrigidRogue.MonoGame.Core.UserInterface;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Views
{
    [ActionMap(Name = "Previous World Builder Step", DefaultKey = Keys.Left)]
    public class PreviousWorldBuilderStepRequest : IRequest
    {
    }
}