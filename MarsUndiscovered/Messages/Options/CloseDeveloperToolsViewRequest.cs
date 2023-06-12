using FrigidRogue.MonoGame.Core.UserInterface;

using MediatR;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Close Developer Tools", DefaultKey = Keys.Escape)]
    public class CloseDeveloperToolsViewRequest : IRequest
    {
    }
}