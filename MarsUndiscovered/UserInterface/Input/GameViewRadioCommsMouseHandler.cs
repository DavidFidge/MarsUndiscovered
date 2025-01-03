﻿using FrigidRogue.MonoGame.Core.UserInterface;
using MarsUndiscovered.Messages;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Input
{
    public class GameViewRadioCommsMouseHandler : BaseMouseHandler
    {
        public override void HandleLeftMouseClick(MouseState mouseState, MouseState origin)
        {
            base.HandleLeftMouseClick(mouseState, origin);
            
            Mediator.Send(new EndRadioCommsRequest());
        }
    }
}