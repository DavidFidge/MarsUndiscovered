using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Extensions;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;

using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Game.Commands
{
    public class LineAttackCommand : BaseAttackCommand<LineAttackCommandSaveData>
    {
        public Actor Source => GameWorld.GameObjects[_data.SourceId] as Actor;
        public List<Point> Path => _data.Path;
        
        public LineAttackCommand(IGameWorld gameWorld) : base(gameWorld)
        {
        }
        
        protected override CommandResult ExecuteInternal()
        {
            if (Source.LineAttack == null)
                throw new Exception("Object does not have a line attack");

            var targets = GetTargets();
            
            if (targets.IsEmpty())
                return CommandResult.Exception(this, "No targets found for LineAttack");

            var commandResult = CommandResult.Success(this, new List<string>(targets.Count));

            foreach (var target in targets)
            {
                var damage = Source.LineAttack.Roll();

                var lineAttackCommandRestore = new AttackRestoreData
                {
                    Id = target.ID,
                    Damage = damage,
                    Health = target.Health,
                    Shield = target.Shield
                };
                
                _data.LineAttackCommandRestore.Add(lineAttackCommandRestore);

                target.ApplyDamage(damage);

                SetHuntingIfAttackedByPlayer(Source, target);
                
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
            foreach (var restore in _data.LineAttackCommandRestore)
            {
                ((Actor)GameWorld.GameObjects[restore.Id]).Health = restore.Health;
                ((Actor)GameWorld.GameObjects[restore.Id]).Shield = restore.Shield;
            }
        }

        public void Initialise(Actor source, List<Point> path)
        {
            _data.SourceId = source.ID;
            _data.Path = path;
        }

        public IList<Actor> GetTargets()
        {
            return Path
                .Skip(1)
                .Where(p => Source.CurrentMap.Contains(p))
                .Select(p => Source.CurrentMap.GetObjectAt<Actor>(p))
                .Where(p => p != null)
                .ToList();
        }
    }
}
