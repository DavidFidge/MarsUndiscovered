using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Services;

using MarsUndiscovered.Game.Components.SaveData;
using MarsUndiscovered.Interfaces;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Game.Components;

public class ActorAllegianceCollection : Dictionary<AllegianceCategory, ActorAllegianceItem>
{
    public void Initialise()
    {
        var player = new ActorAllegianceItem { AllegianceCategory = AllegianceCategory.Player };
        var monsters = new ActorAllegianceItem { AllegianceCategory = AllegianceCategory.Monsters };
        var miners = new ActorAllegianceItem { AllegianceCategory = AllegianceCategory.Miners };

        player.Relationships.Add(monsters, ActorAllegianceState.Enemy);
        player.Relationships.Add(miners, ActorAllegianceState.Neutral);
        player.Relationships.Add(player, ActorAllegianceState.Ally);

        monsters.Relationships.Add(player, ActorAllegianceState.Enemy);
        monsters.Relationships.Add(miners, ActorAllegianceState.Enemy);
        monsters.Relationships.Add(monsters, ActorAllegianceState.Friendly);

        miners.Relationships.Add(player, ActorAllegianceState.Neutral);
        miners.Relationships.Add(monsters, ActorAllegianceState.Enemy);
        miners.Relationships.Add(miners, ActorAllegianceState.Ally);

        Add(player.AllegianceCategory, player);
        Add(monsters.AllegianceCategory, monsters);
        Add(miners.AllegianceCategory, miners);
    }

    public ActorAllegianceState RelationshipTo(AllegianceCategory source, AllegianceCategory target)
    {
        return this[source].Relationships[this[target]];
    }

    public ActorAllegianceState RelationshipTo(Actor source, Actor target)
    {
        return this[source.AllegianceCategory].Relationships[this[target.AllegianceCategory]];
    }

    public void Change(AllegianceCategory source, AllegianceCategory target, ActorAllegianceState newState, bool twoWay = false)
    {
        this[source].Relationships[this[target]] = newState;
        
        if (twoWay)
            this[target].Relationships[this[source]] = newState;
    }

    public void LoadState(ISaveGameService saveGameService, IGameWorld gameWorld)
    {
        var state = saveGameService.GetFromStore<ActorAllegianceSaveData>().State;

        foreach (var item in state.Items)
        {
            // Do checks for save game compatibility
            if (this.ContainsKey(item.Source))
            {
                if (this[item.Source].Relationships.ContainsKey(this[item.Target]))
                {
                    Change(item.Source, item.Target, item.State);
                }
            }
        }
    }

    public void SaveState(ISaveGameService saveGameService, IGameWorld gameWorld)
    {
        var saveState = new ActorAllegianceSaveData();

        foreach (var item in this)
        {
            foreach (var subItem in item.Value.Relationships)
            {
                saveState.Items.Add(new ActorAllegianceSaveDataItem
                {
                    Source = item.Value.AllegianceCategory,
                    Target = subItem.Key.AllegianceCategory,
                    State = subItem.Value
                });
            }
        }

        saveGameService.SaveToStore(new Memento<ActorAllegianceSaveData>(saveState));
    }
}
