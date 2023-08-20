using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Extensions;
using MarsUndiscovered.Interfaces;

using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Game.Commands
{
    public class LightningAttackCommand : BaseAttackCommand<LightningAttackCommandSaveData>
    {
        private List<AttackRestoreData> _targetHitDetails = new List<AttackRestoreData>();
        private IList<Actor> _targets;
        public IList<Actor> Targets => _targets;

        public Actor Source { get; private set; }
        public List<Point> Path { get; private set; }

        public LightningAttackCommand(IGameWorld gameWorld) : base(gameWorld)
        {
        }

        public override IMemento<LightningAttackCommandSaveData> GetSaveState()
        {
            var memento = new Memento<LightningAttackCommandSaveData>(new LightningAttackCommandSaveData());
            base.PopulateSaveState(memento.State);

            memento.State.SourceId = Source.ID;
            memento.State.Path = Path.ToList();
            memento.State.LineAttackCommandRestore = _targetHitDetails.ToList();

            return memento;
        }

        public override void SetLoadState(IMemento<LightningAttackCommandSaveData> memento)
        {
            PopulateLoadState(memento.State);

            Source = (Actor)GameWorld.GameObjects[memento.State.SourceId];
            Path = memento.State.Path.ToList();
            _targets = GetTargets(Source, Path);
            _targetHitDetails = memento.State.LineAttackCommandRestore.ToList();
        }
        
        protected override CommandResult ExecuteInternal()
        {
            if (Source.LightningAttack == null)
                throw new Exception("Object does not have a lightning attack");

            var damage = Source.LightningAttack.Damage;

            var commandResult = CommandResult.Success(this, new List<string>(_targets.Count));

            foreach (var target in _targets)
            {
                var lineAttackCommandRestore = new AttackRestoreData
                {
                    Damage = damage,
                    Health = target.Health,
                    Shield = target.Shield
                };
                
                _targetHitDetails.Add(lineAttackCommandRestore);
                
                target.ApplyDamage(damage);

                var message = $"{Source.NameSpecificArticleUpperCase} zapped {target.NameSpecificArticleLowerCase}";
                commandResult.Messages.Add(message);
                
                SetHuntingIfAttackedByPlayer(Source, target);
                
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

        public void Initialise(Actor source, Point targetPoint)
        {
            var lightningAttackPath = Lines.GetLine(source.Position, targetPoint).ToList();

            lightningAttackPath = lightningAttackPath
                .TakeWhile(p => p == source.Position || (source.CurrentMap.Contains(targetPoint) && source.CurrentMap.GetObjectsAt(p).All(o => o.IsGameObjectStrikeThrough())))
                .ToList();

            Source = source;
            Path = lightningAttackPath;
            _targets = GetTargets(Source, Path);
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
