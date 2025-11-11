using FrigidRogue.MonoGame.Core.Components;
using GoRogue.GameFramework;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.ViewMessages;
using MarsUndiscovered.Interfaces;
using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Commands
{
    public class ExplodeTileCommand : BaseMarsGameActionCommand
    {
        public Point Point { get; set; }
        public MarsGameObject Source { get; set; }
        public Actor ActorSource { get; set; }
        public int Damage { get; set; }

        public ExplodeTileCommand(IGameWorld gameWorld) : base(gameWorld)
        {
        }

        public void Initialise(Point point, int damage, MarsGameObject source = null)
        {
            Damage = damage;
            Point = point;
            ActorSource = source as Actor;
            Source = source;
        }

        protected override CommandResult ExecuteInternal()
        {
            var subsequentCommands = new List<BaseGameActionCommand>();

            var gameObjects = GameWorld.CurrentMap.GetObjectsAt(Point);
            var commandResult = CommandResult.Success(this, new List<string>());

            foreach (var gameObject in gameObjects)
            {
                if (gameObject is Wall wall)
                {
                    GameWorld.CurrentMap.MarsMap().CreateFloor(
                        FloorType.RockFloor,
                        Point,
                        GameWorld.GameObjectFactory);
                }
                else if (gameObject is Actor actor)
                {
                    actor.ApplyDamage(Damage);
                    
                    var sourceSentence = ActorSource?.GetSentenceName(false, true) ?? "An explosion";
                    
                    var message = $"{sourceSentence} blasts {actor.GetSentenceName(true, false)} for {Damage} dammage";
                    commandResult.Messages.Add(message);
                    
                    if (actor.Health <= 0)
                    {
                        var deathCommand = GameWorld.CommandCollection.CreateCommand<DeathCommand>(GameWorld);

                        sourceSentence = "an explosion";

                        if (Source != null && Source is Actor actorSource)
                            sourceSentence = actorSource.GetSentenceName(true, true);
                        
                        deathCommand.Initialise(actor, sourceSentence);
                        commandResult.SubsequentCommands.Add(deathCommand);
                    }
                }
                else if (gameObject is Door door)
                {
                    GameWorld.CurrentMap.RemoveEntity(door);
                }
                else if (gameObject is Feature feature)
                {
                    GameWorld.CurrentMap.RemoveEntity(feature);
                }
            }

            if (ContextualEnhancedRandom.FromGlobalRandom.NextBool(nameof(ExplodeTileCommand)))
            {
                GameWorld.SpawnFeature(new SpawnFeatureParams()
                    .WithFeatureType(FeatureType.RubbleType)
                    .OnMap(GameWorld.CurrentMap.Id)
                    .AtPosition(Point));
            }

            if (!commandResult.Messages.Any())
            {
                commandResult.Messages.Add("There was an explosion nearby");
            }

            Mediator.Publish(new MapTileChangedNotification(Point));

            return Result(commandResult);
        }
    }
}
