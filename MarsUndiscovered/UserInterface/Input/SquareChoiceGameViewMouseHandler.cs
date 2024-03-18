using MarsUndiscovered.Messages;

using FrigidRogue.MonoGame.Core.UserInterface;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Input
{
    public class SquareChoiceGameViewMouseHandler : BaseMouseHandler
    {
        public override void HandleLeftMouseClick(MouseState mouseState, MouseState origin)
        {
            Mediator.Send(new LeftClickSquareChoiceGameViewRequest(mouseState.X, mouseState.Y));
        }
    }
}