using FrigidRogue.MonoGame.Core.Interfaces.Components;

using GoRogue.GameFramework;

using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Components.Factories
{
    public interface IGameObjectFactory : ISaveable
    {
        void Initialise(IGameWorld gameWorld);
        uint LastId { get; }
        IDictionary<uint, IGameObject> GameObjects { get; }
        Player CreatePlayer();
        Player CreatePlayer(uint id);
        Wall CreateWall();
        Wall CreateWall(uint id);
        Floor CreateFloor();
        Floor CreateFloor(uint id);
        Monster CreateMonster();
        Monster CreateMonster(uint id);
        Item CreateItem();
        Item CreateItem(uint id);
        MapExit CreateMapExit();
        MapExit CreateMapExit(uint id);
        Ship CreateShip();
        Ship CreateShip(uint id);
    }
}
