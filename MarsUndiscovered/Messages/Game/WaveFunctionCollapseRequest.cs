using FrigidRogue.MonoGame.Core.UserInterface;

using MediatR;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Wave Function Collapse", DefaultKey = Keys.F)]
    
    public class WaveFunctionCollapseRequest : IRequest
    {
    }
}