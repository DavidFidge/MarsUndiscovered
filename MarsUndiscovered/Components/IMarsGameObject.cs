using System;
using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;

using GoRogue.GameFramework;

using MarsUndiscovered.Components.SaveData;

namespace MarsUndiscovered.Components
{
    public interface IMarsGameObject<T> : IGameObject, IBaseComponent, IMementoState<T> where T : GameObjectSaveData
    {
    }
}