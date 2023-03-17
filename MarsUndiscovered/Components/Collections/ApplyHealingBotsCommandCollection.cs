using MarsUndiscovered.Commands;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Components
{
    public class ApplyHealingBotsCommandCollection : BaseCommandCollection<ApplyHealingBotsCommand, ApplyHealingBotsCommandSaveData>
    {
        private readonly ICommandFactory _commandFactory;

        public ApplyHealingBotsCommandCollection(ICommandFactory commandFactory, IGameWorld gameWorld) : base(gameWorld)
        {
            _commandFactory = commandFactory;
        }

        protected override ApplyHealingBotsCommand Create()
        {
            return _commandFactory.CreateApplyHealingBotsCommand(GameWorld);
        }
    }
}