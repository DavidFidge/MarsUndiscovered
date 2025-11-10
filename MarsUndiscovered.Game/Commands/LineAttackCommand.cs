using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Extensions;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Game.Commands
{
    public class LineAttackCommand : BaseAttackCommand
    {
        public Actor Source { get; set; }
        public List<Point> Path { get; set; }
        
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

                var attackData = new AttackData
                {
                    Id = target.ID,
                    Damage = damage,
                    Health = target.Health,
                    Shield = target.Shield
                };
                

                target.ApplyDamage(damage);

                SetHuntingIfAttackedByPlayer(Source, target);
                
                var message = $"{Source.GetSentenceName(false, false)} hit {target.GetSentenceName(true, false)}";
                commandResult.Messages.Add(message);

                if (target.Health <= 0)
                {
                    var deathCommand = GameWorld.CommandCollection.CreateCommand<DeathCommand>(GameWorld);
                    deathCommand.Initialise(target, Source.GetSentenceName(true, true));
                    commandResult.SubsequentCommands.Add(deathCommand);
                }
            }

            return Result(commandResult);
        }

        public void Initialise(Actor source, List<Point> path)
        {
            Source = source;
            Path = path;
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
