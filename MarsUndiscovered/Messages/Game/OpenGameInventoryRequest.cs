using FrigidRogue.MonoGame.Core.UserInterface;
using MediatR;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Open Game Inventory", DefaultKey = Keys.I)]
    public class OpenGameInventoryRequest : IRequest
    {
    }
}