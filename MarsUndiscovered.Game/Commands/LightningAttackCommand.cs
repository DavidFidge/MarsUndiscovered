using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Extensions;
using MarsUndiscovered.Interfaces;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Game.Commands
{
    public class LightningAttackCommand : BaseAttackCommand<LightningAttackCommandSaveData>
    {
        public Actor Source => GameWorld.GameObjects[_data.SourceId] as Actor; 
        public List<Point> Path => _data.Path;

        public LightningAttackCommand(IGameWorld gameWorld) : base(gameWorld)
        {
        }
        
        protected override CommandResult ExecuteInternal()
        {
            if (Source.LightningAttack == null)
                throw new Exception("Object does not have a lightning attack");

            var targets = GetTargets();

            var damage = Source.LightningAttack.Damage;

            var commandResult = CommandResult.Success(this, new List<string>(targets.Count));
            
            foreach (var target in targets)
            {
                var lineAttackCommandRestore = new AttackData
                {
                    Id = target.ID,
                    Damage = damage,
                    Health = target.Health,
                    Shield = target.Shield
                };
                
                _data.LightningAttackData.Add(lineAttackCommandRestore);
                
                target.ApplyDamage(damage);

                var message = $"{Source.GetSentenceName(false, false)} zapped {target.GetSentenceName(true, false)}";
                commandResult.Messages.Add(message);
                
                SetHuntingIfAttackedByPlayer(Source, target);
                
                if (target.Health <= 0)
                {
                    var deathCommand = CommandCollection.CreateCommand<DeathCommand>(GameWorld);
                    deathCommand.Initialise(target, Source.GetSentenceName(false, true));
                    commandResult.SubsequentCommands.Add(deathCommand);
                }
            }

            return Result(commandResult);
        }

        public void Initialise(Actor source, Point targetPoint)
        {
            var lightningAttackPath = Lines.GetLine(source.Position, targetPoint).ToList();

            lightningAttackPath = lightningAttackPath
                .TakeWhile(p => p == source.Position || (source.CurrentMap.Contains(targetPoint) && source.CurrentMap.GetObjectsAt(p).All(o => o.IsGameObjectStrikeThrough())))
                .ToList();

            _data.SourceId = source.ID;
            _data.Path = lightningAttackPath;
        }

        public IList<Actor> GetTargets()
        {
            return Path
                .Skip(1)
                .Select(p => Source.CurrentMap.GetObjectAt<Actor>(p))
                .Where(p => p != null)
                .ToList();
        }
    }
}
