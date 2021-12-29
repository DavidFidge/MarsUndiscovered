using AutoMapper;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;
using MarsUndiscovered.Components.SaveData;

namespace MarsUndiscovered.Components
{
    public class Wall : Terrain<WallSaveData>
    {
        public Wall() : base(false, false)
        {
        }

        public Wall(uint id) : base(false, false)
        {
        }
    }
}