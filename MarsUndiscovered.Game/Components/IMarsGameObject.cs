using FrigidRogue.MonoGame.Core.Components;

using GoRogue.GameFramework;

namespace MarsUndiscovered.Game.Components
{
    public interface IMarsGameObject : IGameObject, IBaseComponent
    {
        public char AsciiCharacter { get; }
    }
}