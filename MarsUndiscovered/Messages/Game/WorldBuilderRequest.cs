using FrigidRogue.MonoGame.Core.UserInterface;

using MediatR;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "World Builder", DefaultKey = Keys.W)]
    
    public class WorldBuilderRequest : IRequest
    {
        public ulong? Seed { get; set; } = null;
    }
}