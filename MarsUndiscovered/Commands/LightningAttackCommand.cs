
using System;
using System.Collections.Generic;
using System.Linq;

using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using GoRogue;

using MarsUndiscovered.Components;
using MarsUndiscovered.Interfaces;

using SadRogue.Primitives;

namespace MarsUndiscovered.Commands
{
    public class LightningAttackCommand : BaseAttackCommand<LightningAttackCommandSaveData>
    {
        public Actor Source { get; private set; }
        public IList<Actor> Targets { get; private set; }

        private int _damage;
        public List<Point> Path { get; private set; }

        public LightningAttackCommand(IGameWorld gameWorld) : base(gameWorld)
        {
        }

        public override IMemento<LightningAttackCommandSaveData> GetSaveState()
        {
            var memento = new Memento<LightningAttackCommandSaveData>(new LightningAttackCommandSaveData());
            base.PopulateSaveState(memento.State);

            memento.State.SourceId = Source.ID;
            memento.State.TargetIds = Targets.Select(t => t.ID).ToList();
            memento.State.Path = Path.ToList();

            return memento;
        }

        public override void SetLoadState(IMemento<LightningAttackCommandSaveData> memento)
        {
            PopulateLoadState(memento.State);

            Source = (Actor)GameWorld.GameObjects[memento.State.SourceId];
            Targets = memento.State.TargetIds.Select(t => (Actor)GameWorld.GameObjects[t]).ToList();
            Path = memento.State.Path.ToList();
        }

        protected override CommandResult ExecuteInternal()
        {
            if (Source.LightningAttack == null)
                throw new Exception("Object does not have a lightning attack");

            _damage = Source.LightningAttack.Damage;

            var commandResult = CommandResult.Success(this, new List<string>(Targets.Count));

            foreach (var target in Targets)
            {
                target.Health -= _damage;

                var message = $"{Source.NameSpecificArticleUpperCase} zapped {target.NameSpecificArticleLowerCase}";
                commandResult.Messages.Add(message);

                if (target.Health <= 0)
                {
                    var deathCommand = CommandFactory.CreateDeathCommand(GameWorld);
                    deathCommand.Initialise(target, Source.NameGenericArticleLowerCase);
                    commandResult.SubsequentCommands.Add(deathCommand);
                }
            }

            return Result(commandResult);
        }

        protected override void UndoInternal()
        {
            foreach (var target in Targets)
            {
                target.Health += _damage;
            }
        }

        public void Initialise(Actor source, Point targetPoint)
        {
            var lightningAttackPath = Lines.Get(source.Position, targetPoint, Lines.Algorithm.BresenhamOrdered).ToList();

            Source = source;

            Path = lightningAttackPath;

            Targets = lightningAttackPath
                .Skip(1)
                .Select(p => source.CurrentMap.GetObjectAt<Actor>(p))
                .Where(p => p != null)
                .ToList();
        }
    }
}
