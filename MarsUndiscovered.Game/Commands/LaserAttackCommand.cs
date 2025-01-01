using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Extensions;
using MarsUndiscovered.Interfaces;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Game.Commands
{
    public class LaserAttackCommand : BaseAttackCommand<LaserAttackCommandSaveData>
    {
        public Actor Source => GameWorld.GameObjects[_data.SourceId] as Actor; 
        public List<Point> Path => _data.Path;

        public LaserAttackCommand(IGameWorld gameWorld) : base(gameWorld)
        {
        }
        
        public void Initialise(Actor source, Point targetPoint)
        {
            var laserAttackPath = Lines.GetLine(source.Position, targetPoint).ToList();

            laserAttackPath = laserAttackPath
                .TakeWhile(p => p == source.Position || (source.CurrentMap.Contains(targetPoint) && source.CurrentMap.GetObjectsAt(p).All(o => o.IsGameObjectStrikeThrough())))
                .ToList();

            _data.SourceId = source.ID;
            _data.Path = laserAttackPath;
        }
        
        protected override CommandResult ExecuteInternal()
        {
            if (Source.LaserAttack == null)
                throw new Exception("Object does not have a laser attack");

            var targets = GetTargets();

            var damage = Source.LaserAttack.Damage;

            var commandResult = CommandResult.Success(this, new List<string>(targets.Count));
            
            foreach (var target in targets)
            {
                var lineAttackCommandRestore = new AttackRestoreData
                {
                    Id = target.ID,
                    Damage = damage,
                    Health = target.Health,
                    Shield = target.Shield
                };
                
                _data.LaserAttackCommandRestore.Add(lineAttackCommandRestore);
                
                target.ApplyDamage(damage);

                var message = $"{Source.GetSentenceName(false, false)} blasted {target.GetSentenceName(true, false)}";
                commandResult.Messages.Add(message);
                
                SetHuntingIfAttackedByPlayer(Source, target);
                
                if (target.Health <= 0)
                {
                    var deathCommand = CommandCollection.CreateCommand<DeathCommand>(GameWorld);
                    deathCommand.Initialise(target, Source.GetSentenceName(true, true));
                    commandResult.SubsequentCommands.Add(deathCommand);
                }
            }

            return Result(commandResult);
        }

        protected override void UndoInternal()
        {
            foreach (var restore in _data.LaserAttackCommandRestore)
            {
                ((Actor)GameWorld.GameObjects[restore.Id]).Health = restore.Health;
                ((Actor)GameWorld.GameObjects[restore.Id]).Shield = restore.Shield;
            }
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
