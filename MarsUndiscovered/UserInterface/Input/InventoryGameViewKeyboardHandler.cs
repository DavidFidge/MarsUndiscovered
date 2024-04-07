using InputHandlers.Keyboard;

using MarsUndiscovered.Messages;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Input
{
    public class InventoryGameViewKeyboardHandler : BaseInventoryViewKeyboardHandler
    {
        public override void HandleKeyboardKeyDown(Keys[] keysDown, Keys keyInFocus, KeyboardModifier keyboardModifier)
        {
            if (ActionMap.ActionIs<CloseGameInventoryContextRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new CloseGameInventoryContextRequest());

            if (ActionMap.ActionIs<InventoryItemSelectionRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new InventoryItemSelectionRequest(keyInFocus));
            
            if (ActionMap.ActionIs<AssignHotBarItemRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new AssignHotBarItemRequest(keyInFocus));
            
            if (ActionMap.ActionIs<InventoryItemSelectionCycleRequest>(keyInFocus, keyboardModifier))
            {
                var actionName = ActionMap.ActionName<InventoryItemSelectionCycleRequest>(keyInFocus, keyboardModifier);
                
                if (actionName == UiConstants.InventoryItemSelectionCycleRequestNext)
                    Mediator.Send(new InventoryItemSelectionCycleRequest(InventoryItemSelectionCycleRequestType.Next));
                else
                    Mediator.Send(new InventoryItemSelectionCycleRequest(InventoryItemSelectionCycleRequestType.Previous));
            }
        }
    }
}