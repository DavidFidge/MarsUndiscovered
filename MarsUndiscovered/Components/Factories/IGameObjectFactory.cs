using System.Collections.Generic;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using GoRogue.GameFramework;

namespace MarsUndiscovered.Components.Factories
{
    public interface IGameObjectFactory : ISaveable
    {
        uint LastId { get; }
        IDictionary<uint, IGameObject> GameObjects { get; }
        public void Reset();

        Player CreatePlayer();
        Player CreatePlayer(uint id);
        Wall CreateWall();
        Wall CreateWall(uint id);
        Floor CreateFloor();
        Floor CreateFloor(uint id);
        Monster CreateMonster();
        Monster CreateMonster(uint id);
    }
}