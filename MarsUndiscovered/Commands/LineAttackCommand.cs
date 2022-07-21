
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
    public class LineAttackCommand : BaseAttackCommand<LineAttackCommandSaveData>
    {
        private IList<Actor> _targets;
        
        public Actor Source { get; private set; }
        public List<Point> Path { get; private set; }
        
        public LineAttackCommand(IGameWorld gameWorld) : base(gameWorld)
        {
        }

        public override IMemento<LineAttackCommandSaveData> GetSaveState()
        {
            var memento = new Memento<LineAttackCommandSaveData>(new LineAttackCommandSaveData());
            base.PopulateSaveState(memento.State);

            memento.State.SourceId = Source.ID;
            memento.State.Path = Path.ToList();

            return memento;
        }

        public override void SetLoadState(IMemento<LineAttackCommandSaveData> memento)
        {
            PopulateLoadState(memento.State);

            Source = (Actor)GameWorld.GameObjects[memento.State.SourceId];
            Path = memento.State.Path.ToList();
            _targets = GetTargets(Source, Path);
        }
        
        protected override CommandResult ExecuteInternal()
        {
            if (Source.LineAttack == null)
                throw new Exception("Object does not have a line attack");

            var commandResult = CommandResult.Success(this, new List<string>(_targets.Count));

            foreach (var target in _targets)
            {
                var attackCommand = CommandFactory.CreateAttackCommand(GameWorld);
                attackCommand.Initialise(Source, target);
                commandResult.SubsequentCommands.Add(attackCommand);
            }

            return Result(commandResult);
        }

        protected override void UndoInternal()
        {
        }

        public void Initialise(Actor source, Point targetPoint)
        {
            var lineAttackPath = Lines.Get(source.Position, targetPoint, Lines.Algorithm.BresenhamOrdered).ToList();

            lineAttackPath = lineAttackPath
                .TakeWhile(p => p == source.Position || (source.CurrentMap.GetObjectAt<Wall>(p) == null && source.CurrentMap.GetObjectAt<Indestructible>(p) == null))
                .ToList();

            Source = source;

            Path = lineAttackPath;

            _targets = GetTargets(source, lineAttackPath);
        }

        private IList<Actor> GetTargets(Actor source, List<Point> lineAttackPath)
        {
            return lineAttackPath
                .Skip(1)
                .Select(p => source.CurrentMap.GetObjectAt<Actor>(p))
                .Where(p => p != null)
                .ToList();
        }
    }
}
