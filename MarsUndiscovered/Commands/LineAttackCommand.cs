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
        private Dictionary<Monster, int> _targetDamage = new Dictionary<Monster, int>();

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
        }
        
        protected override CommandResult ExecuteInternal()
        {
            if (Source.LineAttack == null)
                throw new Exception("Object does not have a line attack");

            var targets = GetTargets(Source, Path);

            if (targets.IsEmpty())
                return CommandResult.Exception(this, "No targets found for LineAttack");

            var commandResult = CommandResult.Success(this, new List<string>(targets.Count));

            foreach (var target in targets)
            {
                var damage = Source.LineAttack.Roll();

                target.Health -= damage;

                var message = $"{Source.NameSpecificArticleUpperCase} hit {target.NameSpecificArticleLowerCase}";
                commandResult.Messages.Add(message);

                if (target.Health <= 0)
                {
                    var deathCommand = CommandFactory.CreateDeathCommand(GameWorld);
                    deathCommand.Initialise(target, Source.NameGenericArticleLowerCase);
                    commandResult.SubsequentCommands.Add(deathCommand);
                }
                
                _targetDamage.Add(target, damage);
            }

            return Result(commandResult);
        }

        protected override void UndoInternal()
        {
            foreach (var monster in _targetDamage)
            {
                monster.Key.Health += monster.Value;
            }
        }

        public void Initialise(Actor source, List<Point> path)
        {
            Source = source;
            Path = path;
        }

        private IList<Monster> GetTargets(Actor source, List<Point> lineAttackPath)
        {
            return lineAttackPath
                .Skip(1)
                .Select(p => source.CurrentMap.GetObjectAt<Monster>(p))
                .Where(p => p != null)
                .ToList();
        }
    }
}
