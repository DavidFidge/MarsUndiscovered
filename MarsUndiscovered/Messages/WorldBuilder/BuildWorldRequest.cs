using FrigidRogue.MonoGame.Core.Components.Mediator;
using FrigidRogue.MonoGame.Core.UserInterface;
using MarsUndiscovered.Game.Components;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Build New World", DefaultKey = Keys.B)]
    public class BuildWorldRequest : IRequest
    {
        public WorldGenerationTypeParams WorldGenerationTypeParams { get; set; }
    }
}