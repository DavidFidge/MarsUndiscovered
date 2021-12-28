using System;
using System.Collections.Generic;
using System.Linq;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using GoRogue.GameFramework;
using MarsUndiscovered.Components.SaveData;

namespace MarsUndiscovered.Components
{
    public class GameObjectCollection : Dictionary<uint, IGameObject>, ISaveable
    {
        public void SaveGame(ISaveGameStore saveGameStore)
        {
            var gameObjects = Values.ToList();

            saveGameStore.SaveToStore<IList<IGameObject>, IList<GameObjectSaveData>>(gameObjects);
        }

        public void LoadGame(ISaveGameStore saveGameStore)
        {
            var gameObjects = saveGameStore.GetFromStore<IList<IGameObject>, IList<GameObjectSaveData>>();

            foreach (var gameObject in gameObjects)
            {
                Add(gameObject.ID, gameObject);
            }
        }
    }
}