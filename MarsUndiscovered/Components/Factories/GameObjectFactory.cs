using System;
using System.Collections.Generic;
using System.Text;

using Castle.MicroKernel;
using Castle.Windsor;

using FrigidRogue.MonoGame.Core.Interfaces.Services;

using MarsUndiscovered.Components.SaveData;

namespace MarsUndiscovered.Components.Factories
{
    public class GameObjectFactory : IGameObjectFactory
    {
        private readonly IWindsorContainer _container;

        public uint NextId { get; set; } = 0;

        private uint GetNextId()
        {
            return ++NextId;
        }

        public GameObjectFactory(IWindsorContainer container)
        {
            _container = container;
        }

        public void Reset()
        {
            NextId = 0;
        }

        public Player CreatePlayer()
        {
            return ResolveWithNextId<Player>();
        }

        public Player CreatePlayer(uint id)
        {
            return ResolveWithGivenId<Player>(id);
        }

        public Wall CreateWall()
        {
            return ResolveWithNextId<Wall>();
        }

        public Wall CreateWall(uint id)
        {
            return ResolveWithGivenId<Wall>(id);
        }

        public Floor CreateFloor()
        {
            return ResolveWithNextId<Floor>();
        }

        public Floor CreateFloor(uint id)
        {
            return ResolveWithGivenId<Floor>(id);
        }

        public Monster CreateMonster()
        {
            return ResolveWithNextId<Monster>();
        }

        public Monster CreateMonster(uint id)
        {
            return ResolveWithGivenId<Monster>(id);
        }

        private T ResolveWithNextId<T>()
        {
            return _container.Resolve<T>(new Arguments { { "id", GetNextId() } });
        }

        private T ResolveWithGivenId<T>(uint id)
        {
            return _container.Resolve<T>(new Arguments { { "id", id } });
        }

        public void SaveGame(ISaveGameStore saveGameStore)
        {
            saveGameStore.SaveToStore<GameObjectFactory, GameObjectFactoryData>(this);
        }

        public void LoadGame(ISaveGameStore saveGameStore)
        {
            saveGameStore.GetFromStore<GameObjectFactory, GameObjectFactoryData>(this);
        }
    }
}
