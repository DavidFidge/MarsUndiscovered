using FrigidRogue.MonoGame.Core.UserInterface;
using MarsUndiscovered.Messages;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Input
{
    public class InventoryReplayViewMouseHandler : BaseMouseHandler
    {
        public override void HandleLeftMouseClick(MouseState mouseState, MouseState origin)
        {
            Mediator.Send(new LeftClickInventoryReplayViewRequest(mouseState.X, mouseState.Y));
        }
    }
}