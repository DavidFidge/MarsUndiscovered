﻿using FrigidRogue.MonoGame.Core.Components.Mediator;
using FrigidRogue.MonoGame.Core.UserInterface;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Open Replay Inventory", DefaultKey = Keys.I)]
    public class OpenReplayInventoryRequest : IRequest
    {
    }
}