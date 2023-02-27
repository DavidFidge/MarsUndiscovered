using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Extensions;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using MarsUndiscovered.Components;
using MarsUndiscovered.Interfaces;

using SadRogue.Primitives;

namespace MarsUndiscovered.Commands
{
    public class LineAttackCommand : BaseAttackCommand<LineAttackCommandSaveData>
    {
        private List<AttackRestoreData> _targetHitDetails = new List<AttackRestoreData>();
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
            memento.State.LineAttackCommandRestore = _targetHitDetails.ToList();

            return memento;
        }

        public override void SetLoadState(IMemento<LineAttackCommandSaveData> memento)
        {
            PopulateLoadState(memento.State);

            Source = (Actor)GameWorld.GameObjects[memento.State.SourceId];
            Path = memento.State.Path.ToList();
            _targetHitDetails = memento.State.LineAttackCommandRestore.ToList();
            _targets = GetTargets(Source, Path);
        }
        
        protected override CommandResult ExecuteInternal()
        {
            if (Source.LineAttack == null)
                throw new Exception("Object does not have a line attack");

            if (_targets.IsEmpty())
                return CommandResult.Exception(this, "No targets found for LineAttack");

            var commandResult = CommandResult.Success(this, new List<string>(_targets.Count));

            foreach (var target in _targets)
            {
                var damage = Source.LineAttack.Roll();

                var lineAttackCommandRestore = new AttackRestoreData
                {
                    Damage = damage,
                    Health = target.Health,
                    Shield = target.Shield
                };
                
                _targetHitDetails.Add(lineAttackCommandRestore);

                target.ApplyDamage(damage);

                var message = $"{Source.NameSpecificArticleUpperCase} hit {target.NameSpecificArticleLowerCase}";
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
            for (var i = 0; i < _targets.Count; i++)
            {
                _targets[i].Health = _targetHitDetails[i].Health;
                _targets[i].Shield = _targetHitDetails[i].Shield;
            }
        }

        public void Initialise(Actor source, List<Point> path)
        {
            Source = source;
            Path = path;
            _targets = GetTargets(Source, Path);
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
