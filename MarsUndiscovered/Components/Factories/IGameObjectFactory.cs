using FrigidRogue.MonoGame.Core.Interfaces.Components;

namespace MarsUndiscovered.Components.Factories
{
    public interface IGameObjectFactory : ISaveable
    {
        uint NextId { get; set; }
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