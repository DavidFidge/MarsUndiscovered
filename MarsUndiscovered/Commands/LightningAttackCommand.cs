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
        private int _damage;
        private IList<Actor> _targets;
        
        public Actor Source { get; private set; }
        public List<Point> Path { get; private set; }

        public IList<Actor> Targets => _targets;

        public LightningAttackCommand(IGameWorld gameWorld) : base(gameWorld)
        {
        }

        public override IMemento<LightningAttackCommandSaveData> GetSaveState()
        {
            var memento = new Memento<LightningAttackCommandSaveData>(new LightningAttackCommandSaveData());
            base.PopulateSaveState(memento.State);

            memento.State.SourceId = Source.ID;
            memento.State.Path = Path.ToList();

            return memento;
        }

        public override void SetLoadState(IMemento<LightningAttackCommandSaveData> memento)
        {
            PopulateLoadState(memento.State);

            Source = (Actor)GameWorld.GameObjects[memento.State.SourceId];
            Path = memento.State.Path.ToList();
            _targets = GetTargets(Source, Path);
        }
        
        protected override CommandResult ExecuteInternal()
        {
            if (Source.LightningAttack == null)
                throw new Exception("Object does not have a lightning attack");

            _damage = Source.LightningAttack.Damage;

            var commandResult = CommandResult.Success(this, new List<string>(_targets.Count));

            foreach (var target in _targets)
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
            foreach (var target in _targets)
            {
                target.Health += _damage;
            }
        }

        public void Initialise(Actor source, Point targetPoint)
        {
            var lightningAttackPath = Lines.Get(source.Position, targetPoint, Lines.Algorithm.BresenhamOrdered).ToList();

            lightningAttackPath = lightningAttackPath
                .TakeWhile(p => p == source.Position || (source.CurrentMap.GetObjectAt<Wall>(p) == null && source.CurrentMap.GetObjectAt<Indestructible>(p) == null))
                .ToList();

            Source = source;

            Path = lightningAttackPath;

            _targets = GetTargets(source, lightningAttackPath);
        }

        private IList<Actor> GetTargets(Actor source, List<Point> lightningAttackPath)
        {
            return lightningAttackPath
                .Skip(1)
                .Select(p => source.CurrentMap.GetObjectAt<Actor>(p))
                .Where(p => p != null)
                .ToList();
        }
    }
}
