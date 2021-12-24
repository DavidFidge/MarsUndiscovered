using FrigidRogue.MonoGame.Core.UserInterface;

using MediatR;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Cancel Custom Game Seed Request", DefaultKey = Keys.Escape)]
    public class CancelCustomGameSeedRequest : IRequest
    {
    }
}