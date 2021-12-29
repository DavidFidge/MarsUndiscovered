using AutoMapper;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;
using MarsUndiscovered.Components.SaveData;

namespace MarsUndiscovered.Components
{
    public class Player : Actor<PlayerSaveData>
    {
        public Player() : base(1, true)
        {
        }

        public Player(uint id) : base(1, true, idGenerator: () => id)
        {
        }
    }
}