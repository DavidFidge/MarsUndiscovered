using FrigidRogue.MonoGame.Core.Components.Mediator;
using FrigidRogue.MonoGame.Core.UserInterface;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "World Builder", DefaultKey = Keys.W)]
    
    public class WorldBuilderRequest : IRequest
    {
        public ulong? Seed { get; set; } = null;
    }
}