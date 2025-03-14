﻿
using FrigidRogue.MonoGame.Core.Components.Mediator;

namespace MarsUndiscovered.Messages
{
    public class RefreshHotBarRequest : IRequest
    {
        public RefreshHotBarRequest()
        {
        }

        public RefreshHotBarRequest(uint itemId)
        {
            ItemId = itemId;
        }

        // If null then refresh all
        public uint? ItemId { get; set; }
    }
}