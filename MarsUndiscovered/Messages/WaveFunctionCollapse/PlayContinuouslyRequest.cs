﻿using FrigidRogue.MonoGame.Core.UserInterface;
using MediatR;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Play Continuously", DefaultKey = Keys.Enter)]
    public class PlayContinuouslyRequest : IRequest
    {
    }
}