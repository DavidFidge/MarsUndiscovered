using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Commands
{
    public class IdentifyItemCommand : BaseMarsGameActionCommand<IdentifyItemCommandSaveData>
    {
        public Item Item { get; private set; }

        public IdentifyItemCommand(IGameWorld gameWorld) : base(gameWorld)
        {
            EndsPlayerTurn = true;
            PersistForReplay = true;
        }

        public void Initialise(Item item)
        {
            Item = item;
        }

        public override IMemento<IdentifyItemCommandSaveData> GetSaveState()
        {
            var memento = new Memento<IdentifyItemCommandSaveData>(new IdentifyItemCommandSaveData());
            base.PopulateSaveState(memento.State);

            memento.State.ItemId = Item.ID;

            return memento;
        }

        public override void SetLoadState(IMemento<IdentifyItemCommandSaveData> memento)
        {
            base.PopulateLoadState(memento.State);

            Item = GameWorld.Items[memento.State.ItemId];
        }

        protected override CommandResult ExecuteInternal()
        {
            var isIdentified = GameWorld.Inventory.IsIdentified(Item);

            if (isIdentified)
            {
                EndsPlayerTurn = false;
                PersistForReplay = false;
                
                return Result(CommandResult.NoMove(this,
                    $"The {GameWorld.Inventory.ItemTypeDiscoveries.GetInventoryDescriptionWithoutPrefix(Item)} is already identified."));
            }

            var undiscoveredDescription = GameWorld.Inventory.ItemTypeDiscoveries.GetUndiscoveredDescription(Item);
            
            GameWorld.Inventory.ItemTypeDiscoveries.SetItemTypeDiscovered(Item);
            Item.ItemDiscovery.IsItemSpecialDiscovered = true;
            Item.ItemDiscovery.IsEnchantLevelDiscovered = true;

            var discoveredDescription = Item.GetDiscoveredDescription(1);
            
            var message =
                $"The {undiscoveredDescription} is a {discoveredDescription}!";
            
            var commandResult = CommandResult.Success(this, message);

            return Result(commandResult);
        }

        protected override void UndoInternal()
        {
        }
    }
}
