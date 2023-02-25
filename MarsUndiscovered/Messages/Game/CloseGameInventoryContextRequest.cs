using FrigidRogue.MonoGame.Core.UserInterface;
using MediatR;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Close Game Inventory Context", DefaultKey = Keys.Escape)]
    public class CloseGameInventoryContextRequest : IRequest
    {
    }
}