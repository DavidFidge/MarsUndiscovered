using System;
using Castle.MicroKernel.ModelBuilder.Descriptors;
using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Commands;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Components.Factories
{
    public class CommandFactory : ICommandFactory
    {
        public IFactory<MoveCommand> MoveCommandFactory { get; set; }
        public IFactory<WalkCommand> WalkCommandFactory { get; set; }
        public IFactory<AttackCommand> AttackCommandFactory { get; set; }

        private T CreateCommand<T, TData>(Func<T> createMethod, IGameWorld gameWorld) where T : BaseMarsGameActionCommand<TData>
        {
            var command = createMethod();
            command.SetGameWorld(gameWorld);
            return command;
        }

        public MoveCommand CreateMoveCommand(IGameWorld gameWorld)
        {
            return CreateCommand<MoveCommand, MoveCommandSaveData>(() => MoveCommandFactory.Create(), gameWorld);
        }

        public WalkCommand CreateWalkCommand(IGameWorld gameWorld)
        {
            return CreateCommand<WalkCommand, WalkCommandSaveData>(() => WalkCommandFactory.Create(), gameWorld);
        }
        public AttackCommand CreateAttackCommand(IGameWorld gameWorld)
        {
            return CreateCommand<AttackCommand, AttackCommandSaveData>(() => AttackCommandFactory.Create(), gameWorld);
        }
    }
}
