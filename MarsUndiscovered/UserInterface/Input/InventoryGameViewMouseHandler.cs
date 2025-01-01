using FrigidRogue.MonoGame.Core.UserInterface;
using MarsUndiscovered.Messages;
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