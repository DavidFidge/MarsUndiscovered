﻿
using FrigidRogue.MonoGame.Core.Components.Mediator;

namespace MarsUndiscovered.Messages
{
    public class LoadGameRequest : IRequest
    {
        public string Filename { get; set; }
    }
}