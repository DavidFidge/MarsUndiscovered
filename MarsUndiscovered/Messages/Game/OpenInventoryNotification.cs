using FrigidRogue.MonoGame.Core.UserInterface;
using MediatR;
using Microsoft.Xna.Framework.Input;

using SadRogue.Primitives;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Open Inventory", DefaultKey = Keys.I)]
    public class OpenInventoryNotification : INotification
    {
    }
}