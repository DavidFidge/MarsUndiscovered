﻿using System;

using MarsUndiscovered.Components.SaveData;

namespace MarsUndiscovered.Commands
{
    public class DropItemSaveData : BaseCommandSaveData
    {
        public uint ItemId { get; set; }
        public uint GameObjectId { get; set; }
    }
}