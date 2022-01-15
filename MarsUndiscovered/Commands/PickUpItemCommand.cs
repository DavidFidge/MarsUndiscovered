using System;
using System.Collections.Generic;
using System.Diagnostics;

using AutoMapper;

using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using GoRogue.GameFramework;

using MarsUndiscovered.Components;

using SadRogue.Primitives;

namespace MarsUndiscovered.Commands
{
    public class PickUpItemCommand : BaseMarsGameActionCommand<PickUpItemSaveData>
    {
        public Item Item { get; private set; }
        public IGameObject GameObject { get; private set; }
        
        public void Initialise(Item item, IGameObject gameObject)
        {
            GameObject = gameObject;
            Item = item;

            Debug.Assert(Item.Position.Equals(GameObject.Position));
        }

        public override IMemento<PickUpItemSaveData> GetSaveState(IMapper mapper)
        {
            return Memento<PickUpItemSaveData>.CreateWithAutoMapper(this, mapper);
        }

        public override void SetLoadState(IMemento<PickUpItemSaveData> memento, IMapper mapper)
        {
            base.SetLoadState(memento, mapper);

            Memento<PickUpItemSaveData>.SetWithAutoMapper(this, memento, mapper);
            GameObject = GameWorld.GameObjects[memento.State.GameObjectId];
        }

        protected override CommandResult ExecuteInternal()
        {
            var player = GameObject as Player;

            if (player == null)
            {
                return Result(CommandResult.Failure(this, "Monsters currently do not pick up items"));
            }

            var itemDescription = GameWorld.Inventory.GetInventoryDescriptionAsSingleItem(Item);
            itemDescription = $"{itemDescription.Substring(0, 1).ToLower()}{itemDescription.Substring(1)}";

            if (!GameWorld.Inventory.CanPickUpItem)
            {
                return Result(CommandResult.Success(this, $"Your inventory is too full to pick up {itemDescription}"));
            }

            GameWorld.Inventory.Add(Item);
            Item.CurrentMap.RemoveEntity(Item);
            Item.Position = Point.None;

            return Result(CommandResult.Success(this, $"You pick up {itemDescription}"));
        }

        protected override void UndoInternal()
        {
            GameWorld.Inventory.Remove(Item);
            Item.Position = GameObject.Position;
            GameObject.CurrentMap.AddEntity(Item);
        }
    }
}
