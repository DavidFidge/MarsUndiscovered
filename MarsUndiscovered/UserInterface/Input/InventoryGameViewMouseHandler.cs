using MarsUndiscovered.Messages;

using FrigidRogue.MonoGame.Core.UserInterface;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Input
{
    public class InventoryGameViewMouseHandler : BaseMouseHandler
    {
        public override void HandleLeftMouseClick(MouseState mouseState, MouseState origin)
        {
            Mediator.Send(new LeftClickInventoryGameViewRequest(mouseState.X, mouseState.Y));
        }
    }
}