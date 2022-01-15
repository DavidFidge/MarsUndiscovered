using FrigidRogue.MonoGame.Core.UserInterface;
using MediatR;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Close Game Inventory", DefaultKey = Keys.Escape)]
    public class CloseGameInventoryRequest : IRequest
    {
    }
}