using FrigidRogue.MonoGame.Core.Components;
using GoRogue.GameFramework;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.ViewMessages;
using MarsUndiscovered.Interfaces;
using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Commands
{
    public class ExplodeTileCommand : BaseMarsGameActionCommand<ExplodeTileCommandSaveData>
    {
        public Point Point => _data.Point;
        public MarsGameObject Source => _data.SourceId == null ? null : GameWorld.GameObjects[_data.SourceId.Value] as MarsGameObject;
        public Actor ActorSource => _data.SourceId == null ? null : GameWorld.GameObjects[_data.SourceId.Value] as Actor;

        public ExplodeTileCommand(IGameWorld gameWorld) : base(gameWorld)
        {
        }

        public void Initialise(Point point, int damage, MarsGameObject source = null)
        {
            _data.Damage = damage;
            _data.Point = point;
            _data.SourceId = source?.ID;
        }

        protected override CommandResult ExecuteInternal()
        {
            var subsequentCommands = new List<BaseGameActionCommand>();

            var gameObjects = GameWorld.CurrentMap.GetObjectsAt(_data.Point);
            var commandResult = CommandResult.Success(this, new List<string>());

            foreach (var gameObject in gameObjects)
            {
                if (gameObject is Wall wall)
                {
                    GameWorld.CurrentMap.MarsMap().CreateFloor(
                        FloorType.RockFloor,
                        _data.Point,
                        GameWorld.GameObjectFactory);
                }
                if (gameObject is Actor actor)
                {
                    actor.ApplyDamage(_data.Damage);
                    
                    var sourceSentence = ActorSource?.GetSentenceName(false, true) ?? "An explosion";
                    
                    var message = $"{sourceSentence} blasts {actor.GetSentenceName(true, false)} for {_data.Damage} dammage";
                    commandResult.Messages.Add(message);
                    
                    if (actor.Health <= 0)
                    {
                        var deathCommand = CommandCollection.CreateCommand<DeathCommand>(GameWorld);

                        sourceSentence = "an explosion";

                        if (Source != null && Source is Actor actorSource)
                            sourceSentence = actorSource.GetSentenceName(true, true);
                        
                        deathCommand.Initialise(actor, sourceSentence);
                        commandResult.SubsequentCommands.Add(deathCommand);
                    }                    
                }

                if (gameObject is Feature feature)
                {
                    GameWorld.CurrentMap.RemoveEntity(feature);
                }
            }
            
            Mediator.Publish(new MapTileChangedNotification(_data.Point));

            return Result(commandResult);
        }
    }
}
