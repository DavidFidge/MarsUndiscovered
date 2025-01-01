using FrigidRogue.MonoGame.Core.Components.Mediator;
using FrigidRogue.MonoGame.Core.UserInterface;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "New Game", DefaultKey = Keys.N)]
    public class NewGameRequest : IRequest
    {
        public ulong? Seed { get; set; } = null;
    }
}