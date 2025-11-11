using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Interfaces;
using Newtonsoft.Json;

namespace MarsUndiscovered.Game.Commands
{
    public abstract class BaseMarsGameActionCommand : BaseGameActionCommand, IBaseMarsGameActionCommand
    {
        public IGameWorld GameWorld { get; private set; }

        public BaseMarsGameActionCommand(IGameWorld gameWorld)
        {
            GameWorld = gameWorld;
        }

        public void ApplyWeaknesses(Actor source, Actor target)
        {
            if (source.CanConcuss)
                target.ApplyConcussion();
        }
    }
}
