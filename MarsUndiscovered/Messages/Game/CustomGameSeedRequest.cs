using FrigidRogue.MonoGame.Core.UserInterface;

using MediatR;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Custom Game Seed Request", DefaultKey = Keys.C)]
    public class CustomGameSeedRequest : IRequest
    {
    }
}